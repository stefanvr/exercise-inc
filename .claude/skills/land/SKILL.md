---
name: land
description: "Loop phase Land. Use only after the user approves landing: reconcile docs, fold history, merge to main with --no-ff, delete WORK.md."
---

# Land

Landing is a product and repository consistency step, not merely a merge. Only
on the user's word, given for this branch: `WORK.md` › Landing approved, or, for
trivial work, said in this session.

## 1. Reconcile

1. Every accepted decision is in its owning document, and its `PROVISIONAL`
   marker is gone.
2. Every deferral has its `DEFERRED` marker and a `doc/deferred.md` row. Every
   rejected proposal has a row.
3. Every accepted learning is written into its owning document.
4. Every remaining `PROVISIONAL` marker was listed at this review and the user
   let it stand. Otherwise stop and ask.
5. The documents describe the delivered system: domain and its Language, app,
   style, architecture, setup, and the README commands, affordances and index.
   Obsolete text is removed, not annotated. A document past its growth limit is
   split (`documents.md` › Growth).
6. Implementation, tests and documents agree.

Commit what this step changes.

## 2. Fold history

The branch's commits describe the work, not its corrections. Execution
corrections fold into the commit they belong to; requirement changes stay as
their own commits.

```sh
# fold the fixup! commits
GIT_SEQUENCE_EDITOR=true git rebase -i --autosquash main
```

Then read `git log --stat main..HEAD`. A correction that was committed without
`--fixup` is folded by hand. Use the abbreviations `git log --oneline` prints:

```sh
# fold correction C into its target T
GIT_SEQUENCE_EDITOR="sed -i -e '/^pick <C> /d' -e '/^pick <T> /a fixup <C>'" \
  git rebase -i main
```

When folding changed what a commit means, reword its message to describe the
commit that remains:

```sh
GIT_SEQUENCE_EDITOR="sed -i 's/^pick <X> /edit <X> /'" git rebase -i main
git commit --amend -m "<message>"
git rebase --continue
```

Report `history rewritten` or `nothing to fold`; a branch with nothing to fold
is the rule succeeding. Never rewrite a branch someone else works on. A branch
already pushed needs a force-push to match: ask first.

## 3. Check

If `main` has moved since the branch started, `git rebase main` first. Then run
the project's full check command from `README.md` on the branch head. It must
pass, and `git status` must be clean.

## 4. Merge

```sh
git switch main
git merge --no-ff work/<slug> -m "Merge work/<slug>: <what landed, in product terms>"
git branch -d work/<slug>
```

The merge commit marks the work as one unit. Push only on the user's word.

## 5. Delete `WORK.md`

Skip this step for trivial work, which has no `WORK.md`. Otherwise copy
`WORK.md` › Next for the report, then delete the file. It was never committed.

## 6. Report

- the merge commit;
- `history rewritten` or `nothing to fold`;
- the open markers left on `main`: `PROVISIONAL` and `DEFERRED` counts;
- the items from `WORK.md` › Next, as candidates for the next work. Choosing
  the next work is the user's.
