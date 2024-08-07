
-- Procedure to retrieve client records with optional filtering, sorting, and paging
CREATE PROCEDURE [dbo].[sp_getAllClients]
    @Filter NVARCHAR(100) = NULL,          -- Optional filter to search clients based on multiple fields
    @SortBy NVARCHAR(50) = NULL,           -- Optional column name to sort the results
    @SortDescending BIT = 0,               -- Optional flag to determine sorting order (0 for ascending, 1 for descending)
    @PageNumber INT = 1,                   -- Page number for pagination
    @PageSize INT = 10                     -- Number of records per page for pagination
AS
BEGIN
    SET NOCOUNT ON;

    -- Declare a variable to build the dynamic SQL query
    DECLARE @SQL NVARCHAR(MAX);
    
    -- Start constructing the SQL query
    SET @SQL = N'SELECT *
                 FROM (
                    SELECT 
                        g.gender_name,
                        c.first_name,
                        c.last_name,
                        c.email,
                        c.personal_id,
                        c.profile_photo,
                        c.mobile_number,
                        -- Combine address fields into a JSON object
                        (
                            SELECT 
                                c.country AS [country],
                                c.city AS [city],
                                c.street AS [street],
                                c.zip_code AS [zip_code]
                            FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
                        ) AS address,
                        -- Combine client account numbers into a JSON array
                        (
                            SELECT 
                                ca.account_number
                            FROM 
                                bp_client_accounts ca
                            WHERE 
                                ca.client_id = c.client_id
                            FOR JSON PATH
                        ) AS client_accounts,
                        ROW_NUMBER() OVER (';

    -- Apply sorting if provided
    IF @SortBy IS NOT NULL
    BEGIN
        SET @SQL = @SQL + 'ORDER BY ' + QUOTENAME(@SortBy, '[');
        IF @SortDescending = 1
        BEGIN
            SET @SQL = @SQL + ' DESC';
        END
    END
    ELSE
    BEGIN
        -- Default sorting if no SortBy is provided
        SET @SQL = @SQL + 'ORDER BY c.client_id'; -- Default sorting column
    END

    SET @SQL = @SQL + ') AS RowNum
                 FROM 
                    bp_clients c
                 INNER JOIN 
                    bp_lookup_gender g ON c.gender_id = g.gender_id';

    -- Apply filtering if provided
    IF @Filter IS NOT NULL
    BEGIN
        SET @SQL = @SQL + ' WHERE c.first_name LIKE @Filter OR 
                             c.last_name LIKE @Filter OR 
                             c.email LIKE @Filter OR 
                             c.personal_id LIKE @Filter OR 
                             c.mobile_number LIKE @Filter OR 
                             c.country LIKE @Filter OR 
                             c.city LIKE @Filter OR 
                             c.street LIKE @Filter OR 
                             c.zip_code LIKE @Filter';
    END

    -- Apply paging
    SET @SQL = @SQL + ') AS Result
                 WHERE RowNum BETWEEN @PageNumber * @PageSize - @PageSize + 1 
                                    AND @PageNumber * @PageSize';

    -- Execute the dynamic SQL query
    EXEC sp_executesql @SQL, 
        N'@Filter NVARCHAR(100), @PageNumber INT, @PageSize INT',
        @Filter, @PageNumber, @PageSize;
END