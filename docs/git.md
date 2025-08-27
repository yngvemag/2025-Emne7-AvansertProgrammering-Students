# Git Command Cheat Sheet

This document provides a comprehensive overview of essential Git commands, with descriptions and examples for each.

## Table of Contents

- [Setup and Configuration](#setup-and-configuration)
- [Basic Commands](#basic-commands)
- [Branching and Merging](#branching-and-merging)
- [Remote Repositories](#remote-repositories)
- [Inspecting Changes](#inspecting-changes)
- [Undoing Changes](#undoing-changes)
- [Stashing Changes](#stashing-changes)
- [Advanced Operations](#advanced-operations)
- [Git Workflow Examples](#git-workflow-examples)
- [Git Flow Best Practices for Teams](#git-flow-best-practices-for-teams)

## Setup and Configuration
_Configure Git with your personal information and preferences to make it work seamlessly with your development environment. This section covers essential setup commands to get you started with Git._

### Initial Setup

```powershell
# Configure user name and email
git config --global user.name "Your Name"
git config --global user.email "your.email@example.com"

# Set default editor
git config --global core.editor "code --wait"  # For VS Code

# Configure line ending preferences
git config --global core.autocrlf true  # For Windows
git config --global core.autocrlf input  # For macOS/Linux

# Display configuration
git config --list
```
<div style="page-break-after:always;"></div>

### Creating Repositories
_Create new Git repositories from scratch or clone existing ones from remote sources. This allows you to start tracking your projects with Git or contribute to existing projects._

```powershell
# Initialize a new repository
git init

# Clone an existing repository
git clone https://github.com/username/repository.git

# Clone specific branch
git clone -b branch-name https://github.com/username/repository.git

# Clone to specific folder
git clone https://github.com/username/repository.git folder-name
```

## Basic Commands
_The fundamental Git commands you'll use daily for checking file status, staging changes, and committing your work to the repository._

### Checking Status
_View the current state of your working directory and staging area. This command shows which changes are tracked, untracked, staged, or unstaged._

```powershell
# Show repository status
git status

# Show status in short format
git status -s
```
<div style="page-break-after:always;"></div>

### Staging Files
_Prepare changes for the next commit by adding them to the staging area. This step allows you to selectively choose which changes will be included in your next commit._

```powershell
# Stage all changes
git add .

# Stage specific file
git add filename.txt

# Stage specific files
git add file1.txt file2.txt

# Stage all files in a directory
git add directory/

# Interactive staging
git add -p filename.txt
```

### Ignoring Files
_Configure Git to automatically exclude certain files from being tracked. This is useful for build artifacts, local configuration files, and sensitive information that should not be committed._

Create a `.gitignore` file in the root of your repository:

```powershell
# Create .gitignore file
touch .gitignore
```

Common patterns to add to `.gitignore`:

```
# OS files
.DS_Store
Thumbs.db

# Editor files
.idea/
.vscode/
*.swp
*.swo

# Build output
/node_modules/
/build/
/dist/
/target/
/bin/
/obj/

# Logs
*.log
npm-debug.log*

# Environment variables
.env
.env.local
```

### Committing Changes
_Record your staged changes in the repository history with a descriptive message. Commits create savepoints that you can return to later if needed._

```powershell
# Commit staged changes
git commit -m "Your commit message"

# Stage all tracked files and commit
git commit -am "Your commit message"

# Amend previous commit
git commit --amend -m "New commit message"

# Commit with a multi-line message
git commit -m "Subject line" -m "Detailed description"
```
<div style="page-break-after:always;"></div>

## Branching and Merging
_Work on multiple features or fixes simultaneously without interfering with each other by creating isolated development lines that can later be combined._

### Branch Management
_Create, list, rename, and delete branches to organize your development workflow efficiently._

```powershell
# List all branches
git branch

# List remote branches
git branch -r

# List all branches (local and remote)
git branch -a

# Create a new branch
git branch branch-name

# Switch to a branch
git checkout branch-name
# Or using newer syntax (Git 2.23+)
git switch branch-name

# Create and switch to a new branch
git checkout -b branch-name
# Or using newer syntax (Git 2.23+)
git switch -c branch-name

# Delete a branch (if merged)
git branch -d branch-name

# Force delete a branch (even if not merged)
git branch -D branch-name

# Rename current branch
git branch -m new-branch-name

# Switch to previous branch
git switch -
```
<div style="page-break-after:always;"></div>

### Merging
_Integrate changes from one branch into another, combining work from different development lines._

```powershell
# Merge a branch into current branch
git merge branch-name

# Merge without fast-forward
git merge --no-ff branch-name

# Abort a merge in progress
git merge --abort
```

### Rebasing
_Reapply commits from one branch onto another, creating a cleaner, linear project history compared to merging._

```powershell
# Rebase current branch onto another branch
git rebase base-branch

# Interactive rebase
git rebase -i base-branch

# Continue rebase after resolving conflicts
git rebase --continue

# Abort rebase
git rebase --abort
```
<div style="page-break-after:always;"></div>

## Remote Repositories
_Interact with other Git repositories hosted on servers, allowing collaboration with other developers._

### Managing Remotes
_Configure connections to remote repositories where you can push your changes or pull updates from._

```powershell
# List remote repositories
git remote -v

# Add a remote repository
git remote add origin https://github.com/username/repository.git

# Add specific repository URL (example for this project)
git remote add origin https://github.com/yngvemag/uia-ikt460-RI-learning-snake-game.git

# Remove a remote
git remote remove origin

# Change remote URL
git remote set-url origin https://github.com/username/new-repository.git
```

### Fetching and Pulling
_Download updates from remote repositories. Fetching retrieves changes without integrating them, while pulling both retrieves and integrates changes._

```powershell
# Fetch changes from remote
git fetch origin

# Fetch changes from all remotes
git fetch --all

# Pull changes from remote (fetch + merge)
git pull origin main
```
<div style="page-break-after:always;"></div>

### Pushing Changes
_Upload your local commits to a remote repository, sharing your changes with others._

```powershell
# Push to remote repository
git push origin main

# Force push (use with caution)
git push -f origin main

# Push all tags to remote
git push --tags origin
```

### Git Push Configuration

This section explains how to configure Git to push to remote repositories without having to specify the remote name and branch each time.

#### Setting Up the Remote URL

```powershell
# Add the remote repository (only needed once)
git remote add origin https://github.com/yngvemag/uia-ikt460-RI-learning-snake-game.git

# Verify the remote was added correctly
git remote -v
```

#### Configure Default Push Behavior

```powershell
# Configure push behavior to automatically push current branch to upstream branch
git config --global push.default upstream

# OR Configure push behavior to automatically push current branch to matching name
git config --global push.default current

# OR Configure push behavior to match local branch to remote branch with same name
git config --global push.default matching
```

#### Set Up Tracking Information

```powershell
# Set upstream for your current branch (example for main branch)
git branch --set-upstream-to=origin/main main

# Or when pushing for the first time, set tracking information
git push -u origin main
```

After setting these configurations, you can simply use:

```powershell
# Push your changes without specifying remote or branch
git push
```

#### Check Your Configuration

```powershell
# Check your git configuration
git config --list

# Look for remote.origin.url and push.default
git config --get remote.origin.url
git config --get push.default
```

### Working with Tags
_Mark specific points in history as important, typically used for release versions. Tags provide a way to reference specific commits with meaningful names._

```powershell
# List all tags
git tag

# Create a lightweight tag
git tag v1.0.0

# Create an annotated tag
git tag -a v1.0.0 -m "Version 1.0.0"

# Create a tag for a specific commit
git tag -a v1.0.0 -m "Version 1.0.0" commit-hash

# Push specific tag to remote
git push origin v1.0.0

# Push all tags to remote
git push --tags origin

# Delete a local tag
git tag -d v1.0.0

# Delete a remote tag
git push --delete origin v1.0.0
```

## Inspecting Changes
_Examine the details of changes in your repository, including differences between files, commits, and branches._

### Viewing Differences
_Compare files, commits, and branches to understand what has changed and who made the changes._

```powershell
# View unstaged changes
git diff

# View staged changes
git diff --staged

# View changes in a specific file
git diff filename.txt

# View differences between branches
git diff branch1..branch2

# View differences between commits
git diff commit1..commit2
```

### Viewing History
_Review the commit history of your repository to track changes over time and see who made what changes when._

```powershell
# View commit history
git log

# View commit history with patches
git log -p

# View commit history with a graph
git log --graph --oneline --decorate

# View abbreviated stats
git log --stat

# View pretty formatted log
git log --pretty=format:"%h - %an, %ar : %s"

# View commits by author
git log --author="User Name"

# View commits in a date range
git log --since=2.weeks

# Show details of a specific commit
git show commit-hash

# Show the changes in a specific commit
git show commit-hash --stat
```

## Undoing Changes
_Fix mistakes and roll back changes when needed. Git provides multiple ways to undo changes depending on where they are in the workflow._

### Discarding Changes
_Remove unwanted modifications in your working directory, discarding changes that haven't been committed._

```powershell
# Discard changes in working directory
git restore .  # Git 2.23+
# Or (older syntax)
git checkout -- .

# Discard changes for a specific file
git restore filename.txt  # Git 2.23+
# Or (older syntax)
git checkout -- filename.txt
```

### Unstaging Changes
_Remove changes from the staging area while keeping them in your working directory for further modifications._

```powershell
# Unstage all changes
git restore --staged .  # Git 2.23+
# Or (older syntax)
git reset HEAD .

# Unstage a specific file
git restore --staged filename.txt  # Git 2.23+
# Or (older syntax)
git reset HEAD filename.txt
```

### Reset Commits
_Move the current branch pointer to a previous state, undoing commits in various ways while allowing you to control what happens to the changes._

```powershell
# Soft reset (keep changes staged)
git reset --soft HEAD~1

# Mixed reset (keep changes unstaged)
git reset HEAD~1

# Hard reset (discard changes)
git reset --hard HEAD~1

# Reset to specific commit
git reset --hard commit-hash
```

### Reverting Commits
_Create new commits that undo the changes made by previous commits, which is safer than resetting when working on shared branches._

```powershell
# Create a new commit that undoes another commit
git revert commit-hash

# Revert without committing
git revert --no-commit commit-hash
```

### Removing Files
_Delete files from your repository using Git commands, which ensures they're properly removed from both the working directory and the repository._

```powershell
# Remove file from both working directory and staging area
git rm filename.txt

# Remove file only from staging area, keep in working directory
git rm --cached filename.txt

# Remove directory
git rm -r directory/
```

## Stashing Changes
_Temporarily shelve changes you've made to your working directory so you can work on something else and come back to them later._

```powershell
# Stash current changes
git stash

# Stash with a message
git stash save "Work in progress for feature X"

# Stash including untracked files
git stash -u

# List stashes
git stash list

# Apply most recent stash without removing it
git stash apply

# Apply specific stash without removing it
git stash apply stash@{2}

# Apply most recent stash and remove it
git stash pop

# View stash contents
git stash show -p stash@{0}

# Remove a specific stash
git stash drop stash@{0}

# Clear all stashes
git stash clear
```

## Advanced Operations
_More sophisticated Git operations for specific scenarios like finding bugs, cleaning up your repository, or managing submodules._

### Finding Bugs with Bisect
_Use binary search to identify which commit introduced a bug, dramatically reducing debugging time for issues that appeared between known good and bad states._

```powershell
# Start bisect session
git bisect start

# Mark current version as bad
git bisect bad

# Mark a known good version
git bisect good v1.0

# Let Git help you find the bad commit
# Git will checkout different commits for you to test
# After testing, mark each as good or bad
git bisect good  # or
git bisect bad

# End bisect session
git bisect reset
```

### Clean Directory
_Remove untracked files and directories from your working directory, helping to clean up your workspace for a fresh start._

```powershell
# Show what would be deleted
git clean -n

# Delete untracked files
git clean -f

# Delete untracked files and directories
git clean -fd

# Remove untracked and ignored files
git clean -fdx
```

### Submodules
_Include and manage external repositories as subdirectories within your main repository, allowing you to include other projects as dependencies._

```powershell
# Add a submodule to your repository
git submodule add https://github.com/username/repo.git path/to/submodule

# Initialize and update submodules
git submodule update --init --recursive

# Update all submodules
git submodule update --remote
```

### Reflog
_Access Git's reference logs to find lost commits, recover deleted branches, or fix mistakes that seem irreversible._

```powershell
# View reference logs
git reflog

# Create a branch from a reflog entry
git checkout -b recovery-branch HEAD@{2}
```
<div style="page-break-after:always;"></div>

## Git Workflow Examples
_Common patterns and procedures for using Git effectively in different scenarios._

### Feature Branch Workflow
_A standard approach for developing features where each feature is developed in its own branch and merged back to the main branch when completed._

```powershell
# 1. Create a feature branch
git checkout -b feature/new-feature main

# 2. Make changes and commit
git add .
git commit -m "Add new feature"

# 3. Update with latest changes from main
git checkout main
git pull
git checkout feature/new-feature
git rebase main

# 4. Push feature branch to remote
git push -u origin feature/new-feature

# 5. Create pull request (via GitHub/GitLab/etc.)

# 6. After PR is merged, clean up
git checkout main
git pull
git branch -d feature/new-feature
```
<div style="page-break-after:always;"></div>

### Hotfix Workflow
_A process for quickly addressing critical bugs in production code by creating special branches directly from the production state._

```powershell
# Create a hotfix branch from production
git checkout -b hotfix/critical-bug main

# Make fixes and commit
git add .
git commit -m "Fix critical bug"

# Push to remote
git push -u origin hotfix/critical-bug

# Create PR and merge

# After merge, clean up
git checkout main
git pull
git branch -d hotfix/critical-bug
git push origin --delete hotfix/critical-bug
```
<div style="page-break-after:always;"></div>

### Resolving Merge Conflicts
_Step-by-step process for handling conflicts that arise when Git can't automatically merge changes from different branches._

```powershell
# 1. During merge or rebase, conflicts may occur

# 2. Identify conflicted files
git status

# Look for markers like <<<<<<< HEAD, =======, >>>>>>> branch-name
# Edit files to resolve conflicts

# 3. After resolving, stage the fixed files
git add resolved-file.txt

# 4. Continue the merge or rebase
git merge --continue
# OR
git rebase --continue
```

### Interactive Adding
_Precisely control what parts of changed files are staged for the next commit, allowing for more granular commits with focused changes._

```powershell
# Start interactive add
git add -p

# Options during interactive add:
# y - stage this hunk
# n - do not stage this hunk
# q - quit; do not stage this hunk or any remaining hunks
# a - stage this hunk and all later hunks in the file
# d - do not stage this hunk or any later hunks in the file
# g - select a hunk to go to
# / - search for a hunk matching pattern
# j - leave this hunk undecided, see next undecided hunk
# J - leave this hunk undecided, see next hunk
# k - leave this hunk undecided, see previous undecided hunk
# K - leave this hunk undecided, see previous hunk
# s - split this hunk into smaller hunks
# e - manually edit the current hunk
# ? - print help
```
<div style="page-break-after:always;"></div>

### Git Large File Storage (LFS)
_An extension to Git that helps manage large binary files more efficiently by storing references to these files rather than the files themselves._

```powershell
# Install Git LFS
git lfs install

# Track large files
git lfs track "*.psd"
git lfs track "*.zip"

# Make sure .gitattributes is tracked
git add .gitattributes

# Use Git normally after setup
git add file.psd
git commit -m "Add design file"
git push origin main
```

### Finding Issues with Bisect
_A practical example of how to use Git's bisect feature to trace down which commit introduced a specific bug or regression._

```powershell
# Start the bisect process
git bisect start

# Mark the current commit as bad
git bisect bad

# Mark a known good version
git bisect good v1.0

# Git will checkout a commit halfway between good and bad
# Test the app at this commit

# If the bug exists here
git bisect bad

# If the bug doesn't exist here
git bisect good

# Continue this process until Git identifies the bad commit
# Git will tell you which commit introduced the bug

# End the bisect session
git bisect reset
```

## Git Flow Best Practices for Teams

Git Flow is a branching model designed to standardize development workflows in team environments. This section outlines the Git Flow methodology and explains why it's beneficial for team collaboration.

### What is Git Flow?

Git Flow is a branching strategy that defines specific branch roles and how they should interact. It provides a structured framework for managing feature development, releases, and hotfixes.

### Key Branches in Git Flow

1. **Main Branch** (`main` or `master`)
   - Contains production-ready code
   - Always deployable
   - Tagged with version numbers for releases

2. **Develop Branch**
   - Main development branch
   - Contains latest delivered development changes
   - Source for feature branches
   - Merged back into main during releases

3. **Feature Branches**
   - Branch from: `develop`
   - Merge back to: `develop`
   - Naming convention: `feature/feature-name`
   - Used for developing new features
   - Isolated environment for specific functionality

4. **Release Branches**
   - Branch from: `develop`
   - Merge back to: `develop` and `main`
   - Naming convention: `release/X.Y.Z`
   - Used to prepare for production releases
   - Only bug fixes, documentation, and release-oriented tasks

5. **Hotfix Branches**
   - Branch from: `main`
   - Merge back to: `develop` and `main`
   - Naming convention: `hotfix/X.Y.Z` or `hotfix/bug-description`
   - Used to quickly fix critical production bugs

<div style="page-break-after:always;"></div>

### Git Flow Commands

```powershell
# Initialize Git Flow in a repository
git flow init

# Start a new feature
git flow feature start feature-name

# Finish a feature (merges to develop)
git flow feature finish feature-name

# Start a release
git flow release start 1.0.0

# Finish a release
git flow release finish 1.0.0

# Start a hotfix
git flow hotfix start 1.0.1

# Finish a hotfix
git flow hotfix finish 1.0.1
```

### Why Use Git Flow for Teams?

1. **Structured Collaboration**
   - Clear roles for different branches
   - Standardized workflow across team members
   - Reduced confusion about where code should be committed

2. **Parallel Development**
   - Multiple features can be developed simultaneously
   - Independent work streams don't interfere with each other
   - Easier to track what's in progress

3. **Release Management**
   - Dedicated branches for release preparation
   - Ability to maintain multiple production versions
   - Clear separation between development and released code

4. **Quality Control**
   - Feature isolation prevents unstable code from affecting others
   - Release branches allow for testing before production
   - Hotfixes provide emergency repairs without disrupting ongoing work

5. **History Preservation**
   - Meaningful branch structure provides context
   - Cleaner, more navigable commit history
   - Feature branches can be preserved for documentation

6. **Risk Reduction**
   - Isolates experimental or risky changes
   - Reduces conflicts between team members
   - Makes it easier to abandon problematic features

7. **Onboarding Benefits**
   - Easier for new team members to understand workflow
   - Consistent process reduces learning curve
   - Clear guidelines for contributing

### Implementing Git Flow in Your Team

1. **Team Agreement**
   - Ensure all team members understand and agree to follow Git Flow
   - Document workflow expectations
   - Provide training if necessary

2. **Integration with CI/CD**
   - Configure CI/CD pipelines to work with your branch structure
   - Automate testing for feature, release, and hotfix branches
   - Set up deployment rules based on branch roles

3. **Code Review Process**
   - Establish pull request requirements before merging features
   - Define who can approve merges to develop and main
   - Set standards for code quality and testing

4. **Git Flow Tools**
   - Consider using Git Flow extensions or tools
   - Many IDEs have Git Flow integration
   - Use visualization tools to help team members understand the workflow

5. **Regular Maintenance**
   - Clean up merged feature branches
   - Maintain discipline with branch naming and roles
   - Periodically review the workflow and adjust as needed
<div style="page-break-after:always;"></div>

### Variations and Adaptations

While Git Flow provides an excellent framework, it can be adapted to suit specific team needs:

- **Simplified Git Flow**: Some teams prefer to omit release branches for continuous delivery environments
- **Trunk-Based Development**: An alternative with shorter-lived feature branches and more frequent integration
- **GitHub Flow**: A simpler workflow focused on feature branches and pull requests

Choose the approach that best fits your team's size, release cadence, and project requirements.

### Best Practices Summary

1. Keep feature branches short-lived and focused
2. Regularly merge from develop to long-running feature branches
3. Write clear commit messages that explain why changes were made
4. Use tags to mark releases in the main branch
5. Delete branches after they're merged
6. Test thoroughly before merging to develop or main
7. Document your team's specific Git Flow implementation
