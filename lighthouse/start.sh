#!/bin/bash

export ACTORSYSTEM=${ACTORSYSTEM:lighthouse}
export CLUSTER_IP=127.0.0.1
export CLUSTER_PORT=8000

mono Lighthouse.exe
