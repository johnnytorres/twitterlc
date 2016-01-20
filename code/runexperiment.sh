#!/usr/bin/env bash

for d in /users/john/downloads/datasets/twitter/jaime*.tweets; do
    # Will print */ if no directories are available
    echo "processing: "$d
    python topic.py $d
done