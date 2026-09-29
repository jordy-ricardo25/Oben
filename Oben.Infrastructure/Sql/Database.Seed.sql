/*
    Oben local seed data.

    This script is intentionally separated from Database.sql so schema creation
    can be executed without inserting demo credentials.
*/

IF NOT EXISTS (SELECT 1 FROM dbo.users WHERE email = N'admin@oben.local')
BEGIN
    INSERT INTO dbo.users (name, email, password)
    VALUES (N'Administrator', N'admin@oben.local', N'admin123');
END;
GO
