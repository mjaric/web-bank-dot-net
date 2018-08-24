using System;
using System.Collections.Generic;
using Akka.Actor;
using Akka.Persistence;
using Shared.Commands;
using Shared.Events;
using Shared.Messages;

namespace AccountsApp.Aggregates
{
    public class AccountActor : ReceivePersistentActor
    {
        public override string PersistenceId => $"accounts-{_accountNumber}";

        private readonly Account _state;
        private readonly string _accountNumber;

        public AccountActor(string accountNumber)
        {
//            JournalPluginId = "akka.persistence.journal.eventstore";
//            SnapshotPluginId = "akka.persistence.journal.eventstore";
            _accountNumber = accountNumber;
            _state = new Account();

            Command<ICommand>(cmd =>
            {
                Log.Info($"Command received {cmd.GetType().FullName}");
                try
                {
                    Handle((dynamic)cmd);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    Sender.Tell(new ErrorCommandResponse(e.Message, e));
                }
                
            });
            

            // IDomainEvents
            Recover<IDomainEvent>(e =>
            {
                _state.Apply((dynamic)e);
                
            });
            
            // Snapshot Offer
            Recover<SnapshotOffer>(offer =>
            {
                /* ignore */
            });

            Recover<RecoveryCompleted>(e => { Log.Info($"Recovery completed, latest event number is {LastSequenceNr}"); });
            // in case we miss something
            RecoverAny(msg => Log.Warning($"Message of {msg.GetType().FullName} but no recovery method is defined!"));
        }
        

        private void Handle(CreateAccount cmd)
        {
            var sender = Sender;
            if (LastSequenceNr > 0)
            {
                throw new AlreadyCreatedException();
            }

            var created = new AccountCreated
            {
                AccountNumber = cmd.AccountNumber,
                InitialFunds = cmd.InitialBalance
            };
            // when last event is persisted, send response
            Persist(created, e =>
            {
                ApplyEvent(e);
                Respond(sender, "Created", new[] {created});
            });
        }

        private void Handle(Deposit cmd)
        {
            var sender = Sender;

            if (cmd.To != _state.AccountNumber)
            {
                throw new NotFoundException("Invalid Account Number");
            }

            if (cmd.Amount <= 0)
            {
                throw new ValidationException("Invalid Deposit Amout, it must be greater than zero!");
            }


            var credited = new AccountCredited
            {
                TransactionId = cmd.TransactionId,
                Amount = cmd.Amount,
                CurrencyDate = cmd.CurrencyDate,
                From = cmd.From,
                To = _accountNumber
            };
            // when last event is persisted, send response
            Persist(credited, e =>
            {
                ApplyEvent(e);
                Respond(sender, "Ok", new[] {credited});
            });
        }

        private void Handle(Withrowal cmd)
        {
            var sender = Sender;

            if (cmd.From != _state.AccountNumber)
            {
                throw new NotFoundException("Invalid Account Number");
            }

            if (cmd.Amount <= 0)
            {
                throw new ValidationException("Withrowal Amount need to be positive value.");
            }

            if (cmd.Amount > _state.AvailableBalance)
            {
                throw new ValidationException("Insuficient funds");
            }

            var withrowal = new AccountDebited
            {
                TransactionId = cmd.TransactionId,
                From = _state.AccountNumber,
                Amount = cmd.Amount,
                CurrencyDate = DateTime.UtcNow,
                To = cmd.To
            };

            Persist(withrowal, e =>
            {
                ApplyEvent(e);
                Respond(sender, "Ok", new[] {withrowal});
            });
        }

        private void Handle(CommitTransaction cmd)
        {
            var sender = Sender;
            if (cmd.AccountNumber != _state.AccountNumber)
            {
                throw new NotFoundException("Invalid Account Number");
            }

            if (!_state.OutstandingTransaction.ContainsKey(cmd.TransactionId))
            {
                // command idempotent, just confirm that it is done
                Respond(sender, "Commited");
                return;
            }

            var amount = _state.OutstandingTransaction[cmd.TransactionId];
            var commit = new AccountDebitCommited
            {
                TransactionId = cmd.TransactionId,
                Amount = amount,
                EndedDate = DateTime.UtcNow
            };

            Persist(commit, e =>
            {
                ApplyEvent(e);
                Respond(sender, "Commited", new[] {commit});
            });
        }

        private void Handle(RollbackTransaction cmd)
        {
            var sender = Sender;
            if (cmd.AccountNumber != _state.AccountNumber)
            {
                throw new NotFoundException("Invalid Account Number");
            }

            if (_state.OutstandingTransaction.ContainsKey(cmd.TransactionId))
            {
                // command idempotent, just confirm that it is done
                // btw, this particular case is the one that should be validate if transaction even exists but before
                // command reaches aggregate. For such purpose we need to use command handlers as facade to domain
                Respond(sender, "Rolled Back");
                return;
            }

            var amount = _state.OutstandingTransaction[cmd.TransactionId];
            var commit = new AccountDebitRolledBack
            {
                TransactionId = cmd.TransactionId,
                AccountNumber = _state.AccountNumber,
                Reason = cmd.Reason,
                Amount = amount
            };

            Persist(commit, e =>
            {
                ApplyEvent(e);
                Respond(sender, "Commited", new[] {commit});
            });
        }

        private static void Respond(ICanTell dest, string message, IEnumerable<IDomainEvent> events = null)
        {
            dest.Tell(new CommandResponse(message, events), ActorRefs.NoSender);
        }

        private static void ErrorResponse(ICanTell dest, string message)
        {
            dest.Tell(new ErrorCommandResponse(message), ActorRefs.NoSender);
        }

        private void ApplyEvent(IDomainEvent @event)
        {
            ReceiveRecover(@event);
//            Log.Info($"LastSequenceNr={LastSequenceNr}\n{_state}");
            // this can be also actor ref of actor that know who should get message
            Context.System.EventStream.Publish(@event);
        }
    }
}