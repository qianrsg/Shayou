# Agent Notes

## Editing Rules

- Always preserve UTF-8 when editing files that contain Chinese text. Do not rewrite such files through a path that may change encoding.
- If terminal output shows garbled Chinese, treat that as a display-encoding issue first. Verify file contents with explicit UTF-8 reads before changing text.

## Change Scope

- Only make the change explicitly requested by the user.
- Do not remove or refactor surrounding architecture layers unless the user asked for that specific structural change.
- When a request is about inlining logic, inline only the target logic. Do not delete unrelated event registration, callback wiring, helper methods, or domain types.

## Recovery Rules

- If a change goes beyond scope, restore the original structure first, then re-apply the minimal requested change.
- When restoring user-facing strings, prefer the exact original text from version control.
