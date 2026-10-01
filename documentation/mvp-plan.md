# Study Application MVP and Development Priorities

## Table of Contents

1. [Foundation: Lock In From Start](#foundation-lock-in-from-start)
2. [Must Ship](#must-ship)
3. [Next](#next)
4. [Only If Time](#only-if-time)
5. [Cut](#cut)

## FOUNDATION: LOCK IN FROM START

1. **WPF + .NET 9:** Build the application in WPF from the beginning. This targets the +2 bonus and avoids any later UI migration.
2. **Entity Framework Core:** Build all persistent data around EF Core from the beginning rather than initially using TXT/JSON files.
3. **Database:** Use SQLite initially, subject to confirming whether it qualifies for the "external database" bonus. Tasks, categories, sessions, profiles, etc. all persist here.
4. **LINQ + Lambda Expressions:** Use LINQ naturally for task filtering/sorting, statistics, category grouping and analytics.
5. **Proper Architecture:** Separate Models, Views, ViewModels/Services and Data responsibilities from the beginning so we satisfy high cohesion/low coupling without refactoring everything later.

## MUST SHIP

1. **Categories / Subjects:** User-created categories for organising tasks and study activity.
2. **Task Management:** Create, edit, delete and complete tasks with categories, deadlines, priorities and statuses, including filtering and sorting.
3. **Focus Sessions / Study Timer:** Run timed study sessions and optionally associate them with a category or task.
4. **Statistics & Study History:** Store study sessions and calculate totals, averages, session counts and category/date breakdowns.
5. **Visual Analytics:** Display study statistics through charts and graphs using WPF data binding.
6. **Calendar / Planner:** Display tasks and deadlines through a calendar interface.

## NEXT

7. **Application Blocking:** Detect and automatically close selected distracting applications during focus sessions. Prototype this early, even though full integration comes later.
8. **Focus Profiles:** Save reusable timer and application-blocking configurations in the database.
9. **Notifications / Reminders:** Notify users about deadlines, scheduled sessions and completed timers.

## ONLY IF TIME

10. **Study Goals:** Set daily, weekly or category-specific study targets and track progress.
11. **Study Streaks:** Track consecutive days of study activity.
12. **Smart Task Prioritisation:** Use a simple sorting/scoring algorithm based on priority, deadline and status.
13. **External API/Tool Integration:** Only if we find something genuinely useful and straightforward that could qualify for bonus credit. Don't force one in.

## CUT

14. **Productivity Prediction / Machine Learning:** Removed following tutor feedback.
15. **LLM Integration:** No useful reason to include it, and it doesn't qualify as implementing ML anyway.