IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
CREATE TABLE [exercises] (
    [exId] int NOT NULL IDENTITY,
    [exerciseType] nvarchar(max) NULL,
    [Price] decimal(18,2) NOT NULL,
    CONSTRAINT [PK_exercises] PRIMARY KEY ([exId])
);

CREATE TABLE [men] (
    [manId] int NOT NULL IDENTITY,
    [name] nvarchar(max) NOT NULL,
    [userName] nvarchar(max) NOT NULL,
    [email] nvarchar(max) NOT NULL,
    [passwordHash] nvarchar(max) NOT NULL,
    [boy] int NOT NULL,
    [wight] int NOT NULL,
    [age] int NOT NULL,
    [number] bigint NOT NULL,
    [whoIam] int NOT NULL,
    CONSTRAINT [PK_men] PRIMARY KEY ([manId])
);

CREATE TABLE [Admins] (
    [manId] int NOT NULL,
    CONSTRAINT [PK_Admins] PRIMARY KEY ([manId]),
    CONSTRAINT [FK_Admins_men_manId] FOREIGN KEY ([manId]) REFERENCES [men] ([manId]) ON DELETE CASCADE
);

CREATE TABLE [Cotches] (
    [manId] int NOT NULL,
    [cotch_status] nvarchar(max) NULL,
    [available_times] nvarchar(max) NULL,
    [ExerciseId] int NULL,
    CONSTRAINT [PK_Cotches] PRIMARY KEY ([manId]),
    CONSTRAINT [FK_Cotches_exercises_ExerciseId] FOREIGN KEY ([ExerciseId]) REFERENCES [exercises] ([exId]),
    CONSTRAINT [FK_Cotches_men_manId] FOREIGN KEY ([manId]) REFERENCES [men] ([manId]) ON DELETE CASCADE
);

CREATE TABLE [notifications] (
    [notId] int NOT NULL IDENTITY,
    [title] nvarchar(max) NOT NULL,
    [msj] nvarchar(max) NOT NULL,
    [date] nvarchar(max) NOT NULL,
    [IsRead] bit NOT NULL,
    [ManId] int NOT NULL,
    CONSTRAINT [PK_notifications] PRIMARY KEY ([notId]),
    CONSTRAINT [FK_notifications_men_ManId] FOREIGN KEY ([ManId]) REFERENCES [men] ([manId]) ON DELETE CASCADE
);

CREATE TABLE [Users] (
    [manId] int NOT NULL,
    [CotchId] int NULL,
    [subscribeStatus] nvarchar(max) NULL,
    [SubscriptionPlan] nvarchar(max) NULL,
    [SubscriptionStartDate] datetime2 NULL,
    [SubscriptionEndDate] datetime2 NULL,
    [exerciseexId] int NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY ([manId]),
    CONSTRAINT [FK_Users_Cotches_CotchId] FOREIGN KEY ([CotchId]) REFERENCES [Cotches] ([manId]),
    CONSTRAINT [FK_Users_exercises_exerciseexId] FOREIGN KEY ([exerciseexId]) REFERENCES [exercises] ([exId]),
    CONSTRAINT [FK_Users_men_manId] FOREIGN KEY ([manId]) REFERENCES [men] ([manId]) ON DELETE CASCADE
);

CREATE TABLE [appointments] (
    [Id] int NOT NULL IDENTITY,
    [UserId] int NOT NULL,
    [CotchId] int NOT NULL,
    [ExerciseId] int NOT NULL,
    [AppointmentDate] nvarchar(max) NULL,
    [Status] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_appointments] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_appointments_Cotches_CotchId] FOREIGN KEY ([CotchId]) REFERENCES [Cotches] ([manId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_appointments_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([manId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_appointments_exercises_ExerciseId] FOREIGN KEY ([ExerciseId]) REFERENCES [exercises] ([exId]) ON DELETE CASCADE
);

CREATE TABLE [dailyGoals] (
    [goalId] int NOT NULL IDENTITY,
    [status] bit NOT NULL,
    [goal] nvarchar(max) NULL,
    [date] nvarchar(max) NOT NULL,
    [UserId] int NULL,
    CONSTRAINT [PK_dailyGoals] PRIMARY KEY ([goalId]),
    CONSTRAINT [FK_dailyGoals_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([manId])
);

CREATE INDEX [IX_appointments_CotchId] ON [appointments] ([CotchId]);

CREATE INDEX [IX_appointments_ExerciseId] ON [appointments] ([ExerciseId]);

CREATE INDEX [IX_appointments_UserId] ON [appointments] ([UserId]);

CREATE INDEX [IX_Cotches_ExerciseId] ON [Cotches] ([ExerciseId]);

CREATE UNIQUE INDEX [IX_dailyGoals_UserId] ON [dailyGoals] ([UserId]) WHERE [UserId] IS NOT NULL;

CREATE INDEX [IX_notifications_ManId] ON [notifications] ([ManId]);

CREATE INDEX [IX_Users_CotchId] ON [Users] ([CotchId]);

CREATE INDEX [IX_Users_exerciseexId] ON [Users] ([exerciseexId]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260124224017_InitialCreate', N'9.0.11');

COMMIT;
GO

