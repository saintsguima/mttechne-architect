#!/usr/bin/env bash
set -euo pipefail

REPO_NAME="cash-flow-architecture-challenge"
GITHUB_USER="SEU_USUARIO_GITHUB"

git init
git add .
git commit -m "Initial architecture challenge solution"
git branch -M main

gh repo create "$GITHUB_USER/$REPO_NAME" --public --source=. --remote=origin --push
