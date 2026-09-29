/*
    Oben database bootstrap script.

    Scope:
    - Creates the minimum tables required by the technical test.
    - Keeps audit responsibility inside SQL Server through triggers.
    - Uses English table and column names consistently.
    - Uses explicit audit value names: old_value and new_value.
*/

IF OBJECT_ID(N'dbo.users', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.users
    (
        id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_users PRIMARY KEY,
        name NVARCHAR(100) NOT NULL,
        email NVARCHAR(256) NOT NULL,
        password NVARCHAR(200) NOT NULL,
        CONSTRAINT UQ_users_email UNIQUE (email)
    );
END;
GO

IF OBJECT_ID(N'dbo.products', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.products
    (
        id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_products PRIMARY KEY,
        name NVARCHAR(200) NOT NULL,
        description NVARCHAR(1000) NULL,
        price DECIMAL(18,2) NOT NULL,
        stock INT NOT NULL,
        CONSTRAINT CK_products_price CHECK (price > 0),
        CONSTRAINT CK_products_stock CHECK (stock >= 0)
    );
END;
GO

IF OBJECT_ID(N'dbo.product_update_audit', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.product_update_audit
    (
        id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_product_update_audit PRIMARY KEY,
        table_name NVARCHAR(128) NOT NULL,
        record_id INT NOT NULL,
        field_name NVARCHAR(128) NOT NULL,
        old_value NVARCHAR(MAX) NULL,
        new_value NVARCHAR(MAX) NULL,
        user_id INT NULL,
        created_at DATETIME2(7) NOT NULL CONSTRAINT DF_product_update_audit_created_at DEFAULT SYSUTCDATETIME()
    );
END;
GO

IF OBJECT_ID(N'dbo.product_delete_audit', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.product_delete_audit
    (
        id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_product_delete_audit PRIMARY KEY,
        table_name NVARCHAR(128) NOT NULL,
        record_id INT NOT NULL,
        old_value NVARCHAR(MAX) NOT NULL,
        user_id INT NULL,
        created_at DATETIME2(7) NOT NULL CONSTRAINT DF_product_delete_audit_created_at DEFAULT SYSUTCDATETIME()
    );
END;
GO

/*
    Captures one row per changed product field.
    The application must set SESSION_CONTEXT(N'UserId') before UPDATE.
*/
CREATE OR ALTER TRIGGER dbo.trg_products_update_audit
ON dbo.products
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @UserId INT = TRY_CONVERT(INT, SESSION_CONTEXT(N'UserId'));

    INSERT INTO dbo.product_update_audit (table_name, record_id, field_name, old_value, new_value, user_id)
    SELECT N'products', i.id, N'name', d.name, i.name, @UserId
    FROM inserted AS i
    INNER JOIN deleted AS d ON d.id = i.id
    WHERE ISNULL(d.name, N'') <> ISNULL(i.name, N'');

    INSERT INTO dbo.product_update_audit (table_name, record_id, field_name, old_value, new_value, user_id)
    SELECT N'products', i.id, N'description', d.description, i.description, @UserId
    FROM inserted AS i
    INNER JOIN deleted AS d ON d.id = i.id
    WHERE ISNULL(d.description, N'') <> ISNULL(i.description, N'');

    INSERT INTO dbo.product_update_audit (table_name, record_id, field_name, old_value, new_value, user_id)
    SELECT N'products', i.id, N'price', CONVERT(NVARCHAR(50), d.price), CONVERT(NVARCHAR(50), i.price), @UserId
    FROM inserted AS i
    INNER JOIN deleted AS d ON d.id = i.id
    WHERE d.price <> i.price;

    INSERT INTO dbo.product_update_audit (table_name, record_id, field_name, old_value, new_value, user_id)
    SELECT N'products', i.id, N'stock', CONVERT(NVARCHAR(50), d.stock), CONVERT(NVARCHAR(50), i.stock), @UserId
    FROM inserted AS i
    INNER JOIN deleted AS d ON d.id = i.id
    WHERE d.stock <> i.stock;
END;
GO

/*
    Captures the deleted product as JSON.
    The application must set SESSION_CONTEXT(N'UserId') before DELETE.
*/
CREATE OR ALTER TRIGGER dbo.trg_products_delete_audit
ON dbo.products
AFTER DELETE
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @UserId INT = TRY_CONVERT(INT, SESSION_CONTEXT(N'UserId'));

    INSERT INTO dbo.product_delete_audit (table_name, record_id, old_value, user_id)
    SELECT
        N'products',
        d.id,
        (
            SELECT
                d.id,
                d.name,
                d.description,
                d.price,
                d.stock
            FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
        ),
        @UserId
    FROM deleted AS d;
END;
GO
