-- Align existing Supabase tables with the current C# models.
-- This keeps the app migration small before doing a deeper domain refactor.

begin;

alter table levels
    alter column price type double precision using coalesce(price::double precision, 0);

alter table schedules
    alter column start_time type text using start_time::text,
    alter column end_time type text using end_time::text;

alter table academic_history
    alter column written_score type double precision using written_score::double precision,
    alter column oral_score type double precision using oral_score::double precision,
    alter column exam_date type text using exam_date::text;

alter table payments
    alter column amount type double precision using amount::double precision,
    alter column is_paid drop default,
    alter column is_paid type integer using case when is_paid then 1 else 0 end,
    alter column is_paid set default 0,
    alter column payment_date type text using payment_date::text;

alter table payments
    drop constraint if exists payments_is_paid_check;

alter table payments
    add constraint payments_is_paid_check check (is_paid in (0, 1));

commit;
