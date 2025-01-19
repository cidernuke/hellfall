
# Project Report
Link: https://app.clickup.com/9012367417/v/dc/8cjvm1t-112/8cjvm1t-4272

# Git

## First time set-up
- Clone the project from gitlab
- If you haven't installed Git LFS already:
    - go to https://git-lfs.com/ and install it
    - Go to your local repo (root hellfall folder) via the terminal or VS-Code
    - Make sure you are on the main branch
    - Run: 
        - `git lfs install`
        - `git pull`
- Open project in Unity
- Finally, create a new branch for the new feature

## Creating new Branch for a feature
- On gitlab, create new branch with meaningful name
- In vs-code, fetch from main
    - terminal: `git fetch`
    - Source control sidebar: click the three dots next to source control dropdown, navigate to
    pull/push, navigate to 'fetch'
- Now, when clicking the branch-name in the bottom-left corner, you should see your newly created branch under 'remote branches'
- select this branch and you're good to go

## Common rules
- if you want to push something on main, you should first discuss it with the others
- if you need access to a feature someone else is working on, you should first consider merging that branch with our own, or create a new branch with both of your features present, before you merge everything to main


## Merge request
- when you are done with your implementation, push your changes, open a merge request on gitlab and inform everyone in the git-merge channel
- in the merge request, link your ticket from clickup
- after a quick review, either the reviewer or you yourself will continue the merge. If you are unsure or have trouble resolving merge conflicts, ask for help!