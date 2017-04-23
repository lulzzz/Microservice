﻿CREATE TABLE [Core].[Batch]
(
	 [Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1)
	,[BatchId] UNIQUEIDENTIFIER NOT NULL
	,[DateFirstRecorded] DATETIME NOT NULL DEFAULT(GETUTCDATE()) 
)
GO
CREATE UNIQUE INDEX [IX_Batch_BatchId] ON [Core].[Batch] ([BatchId]) INCLUDE ([Id])
GO