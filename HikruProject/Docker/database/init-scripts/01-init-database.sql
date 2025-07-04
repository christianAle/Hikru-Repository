-- Oracle Database Initialization Script for Hikru Assessment Management API
-- This script will run automatically when the container starts

-- Create user for the application
ALTER SESSION SET "_ORACLE_SCRIPT"=true;

-- Create hikru_user with necessary privileges
CREATE USER hikru_user IDENTIFIED BY HikruUser123;
GRANT CONNECT, RESOURCE, DBA TO hikru_user;
GRANT CREATE SESSION TO hikru_user;
GRANT CREATE TABLE TO hikru_user;
GRANT CREATE SEQUENCE TO hikru_user;
GRANT CREATE VIEW TO hikru_user;
GRANT CREATE PROCEDURE TO hikru_user;
GRANT UNLIMITED TABLESPACE TO hikru_user;

-- Connect as hikru_user
CONNECT hikru_user/HikruUser123@XE;

-- Create ASSESSMENTS table (using correct spelling)
CREATE TABLE ASSESSMENTS (
    ID NUMBER(10) GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY,
    TITLE VARCHAR2(100) NOT NULL,
    DESCRIPTION VARCHAR2(1000) NOT NULL,
    LOCATION VARCHAR2(200) NOT NULL,
    STATUS NUMBER(1) DEFAULT 0 NOT NULL,
    RECRUITER_ID NUMBER(10) NOT NULL,
    DEPARTMENT_ID NUMBER(10) NOT NULL,
    BUDGET NUMBER(18,2) NOT NULL,
    CLOSING_DATE DATE,
    CREATED_DATE DATE DEFAULT SYSDATE NOT NULL,
    UPDATED_DATE DATE DEFAULT SYSDATE NOT NULL
);

-- Create indexes for better performance
CREATE INDEX IDX_ASSESSMENTS_TITLE ON ASSESSMENTS(TITLE);
CREATE INDEX IDX_ASSESSMENTS_STATUS ON ASSESSMENTS(STATUS);
CREATE INDEX IDX_ASSESSMENTS_RECRUITER_ID ON ASSESSMENTS(RECRUITER_ID);
CREATE INDEX IDX_ASSESSMENTS_DEPARTMENT_ID ON ASSESSMENTS(DEPARTMENT_ID);
CREATE INDEX IDX_ASSESSMENTS_CREATED_DATE ON ASSESSMENTS(CREATED_DATE);

-- Add constraints
ALTER TABLE ASSESSMENTS ADD CONSTRAINT CHK_ASSESSMENTS_STATUS 
    CHECK (STATUS IN (0, 1, 2, 3)); -- 0=Draft, 1=Open, 2=Closed, 3=Archived

ALTER TABLE ASSESSMENTS ADD CONSTRAINT CHK_ASSESSMENTS_BUDGET 
    CHECK (BUDGET > 0);

-- Add comments for documentation
COMMENT ON TABLE ASSESSMENTS IS 'Assessment management table for HR recruitment process';
COMMENT ON COLUMN ASSESSMENTS.ID IS 'Unique identifier for the assessment';
COMMENT ON COLUMN ASSESSMENTS.TITLE IS 'Assessment title (max 100 characters)';
COMMENT ON COLUMN ASSESSMENTS.DESCRIPTION IS 'Assessment description (max 1000 characters)';
COMMENT ON COLUMN ASSESSMENTS.LOCATION IS 'Assessment location';
COMMENT ON COLUMN ASSESSMENTS.STATUS IS 'Assessment status: 0=Draft, 1=Open, 2=Closed, 3=Archived';
COMMENT ON COLUMN ASSESSMENTS.RECRUITER_ID IS 'ID of the recruiter responsible for this assessment';
COMMENT ON COLUMN ASSESSMENTS.DEPARTMENT_ID IS 'ID of the department for this assessment';
COMMENT ON COLUMN ASSESSMENTS.BUDGET IS 'Budget allocated for this assessment';
COMMENT ON COLUMN ASSESSMENTS.CLOSING_DATE IS 'Optional closing date for the assessment';
COMMENT ON COLUMN ASSESSMENTS.CREATED_DATE IS 'Timestamp when the assessment was created';
COMMENT ON COLUMN ASSESSMENTS.UPDATED_DATE IS 'Timestamp when the assessment was last updated';

-- Insert sample data for testing
INSERT INTO ASSESSMENTS (TITLE, DESCRIPTION, LOCATION, STATUS, RECRUITER_ID, DEPARTMENT_ID, BUDGET, CLOSING_DATE)
VALUES 
('Senior Software Developer Assessment', 'Comprehensive technical assessment for senior developer position including coding challenges and system design', 'New York Office', 1, 101, 1, 75000.00, DATE '2025-07-15');

INSERT INTO ASSESSMENTS (TITLE, DESCRIPTION, LOCATION, STATUS, RECRUITER_ID, DEPARTMENT_ID, BUDGET, CLOSING_DATE)
VALUES 
('Marketing Manager Evaluation', 'Strategic marketing assessment focusing on campaign management and analytical skills', 'Los Angeles Office', 1, 102, 2, 65000.00, DATE '2025-07-20');

INSERT INTO ASSESSMENTS (TITLE, DESCRIPTION, LOCATION, STATUS, RECRUITER_ID, DEPARTMENT_ID, BUDGET, CLOSING_DATE)
VALUES 
('Data Scientist Technical Review', 'Advanced data science assessment covering machine learning, statistics, and data visualization', 'Remote', 0, 103, 3, 85000.00, DATE '2025-08-01');

INSERT INTO ASSESSMENTS (TITLE, DESCRIPTION, LOCATION, STATUS, RECRUITER_ID, DEPARTMENT_ID, BUDGET, CLOSING_DATE)
VALUES 
('UX Designer Portfolio Review', 'Design assessment including portfolio review and user experience design challenge', 'San Francisco Office', 2, 104, 4, 70000.00, DATE '2025-06-30');

INSERT INTO ASSESSMENTS (TITLE, DESCRIPTION, LOCATION, STATUS, RECRUITER_ID, DEPARTMENT_ID, BUDGET, CLOSING_DATE)
VALUES 
('Project Manager Leadership Assessment', 'Leadership and project management skills evaluation with scenario-based questions', 'Chicago Office', 1, 105, 5, 80000.00, DATE '2025-07-25');

COMMIT;

-- Create a trigger to automatically update the UPDATED_DATE column
CREATE OR REPLACE TRIGGER TRG_ASSESSMENTS_UPDATED_DATE
    BEFORE UPDATE ON ASSESSMENTS
    FOR EACH ROW
BEGIN
    :NEW.UPDATED_DATE := SYSDATE;
END;
/

-- Display success message
SELECT 'Assessment table created successfully with sample data!' AS MESSAGE FROM DUAL;

EXIT;
