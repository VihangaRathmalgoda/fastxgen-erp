/*
    Post-deployment script: runs after every deployment, so everything here must be re-runnable.
    Use it only for reference data (MERGE by natural key). Include one file per area:
        :r .\ReferenceData\<area>.sql
*/
PRINT N'FastXGen ERP post-deployment complete.';
