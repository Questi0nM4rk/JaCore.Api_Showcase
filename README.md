# JaCore.Api_Showcase

This is a repo, where I will showcase the Api im building for my project.

Here is a SQL create script, that describes the whole database layout:

````SQL
```SQL
-- tables
-- Table: Device
CREATE TABLE Device (
    ID bigint  NOT NULL,
    Name varchar(100)  NOT NULL,
    LocationID bigint  NOT NULL,
    IsDisabled boolean  NOT NULL DEFAULT false,
    DisabledBy varchar(36)  NULL,
    DisabledAt timestamp  NULL,
    IsRemoved boolean  NOT NULL DEFAULT false,
    RemovedAt timestamp  NULL,
    RemovedBy varchar(36)  NULL,
    CreatedAt timestamp  NOT NULL DEFAULT current_timestamp,
    CreatedBy varchar(36)  NOT NULL,
    ModifiedAt timestamp  NOT NULL DEFAULT current_timestamp,
    ModifiedBy varchar(36)  NOT NULL,
    CONSTRAINT Device_IsRemoved_CHK CHECK (((IsRemoved = FALSE) OR (IsRemoved = TRUE AND RemovedBy IS NOT NULL AND RemovedAt IS NOT NULL))) NOT DEFERRABLE INITIALLY IMMEDIATE,
    CONSTRAINT Device_IsDisabled_CHK CHECK ((     (IsDisabled = FALSE)     OR     (IsDisabled = TRUE AND DisabledBy IS NOT NULL AND DisabledAt IS NOT NULL) )) NOT DEFERRABLE INITIALLY IMMEDIATE,
    CONSTRAINT Device_pk PRIMARY KEY (ID)
);

CREATE INDEX Device_idx_LocationIndex on Device (LocationID ASC);

-- Table: DeviceCard
CREATE TABLE DeviceCard (
    ID bigint  NOT NULL,
    Device_ID bigint  NOT NULL,
    SerialNumber varchar(20)  NOT NULL,
    ActivationDate timestamp  NOT NULL,
    SupplierID bigint  NOT NULL,
    ServiceID bigint  NOT NULL,
    MetConfirmationID bigint  NOT NULL,
    IsRemoved boolean  NOT NULL DEFAULT false,
    RemovedAt timestamp  NULL,
    RemovedBy varchar(36)  NULL,
    CreatedAt timestamp  NOT NULL DEFAULT current_timestamp,
    CreatedBy varchar(36)  NOT NULL,
    ModifiedAt timestamp  NOT NULL DEFAULT current_timestamp,
    ModifiedBy varchar(36)  NOT NULL,
    CONSTRAINT DeviceCard_Device_ID_Unique UNIQUE (Device_ID) NOT DEFERRABLE  INITIALLY IMMEDIATE,
    CONSTRAINT DeviceCard_IsRemoved_CHK CHECK (((IsRemoved = FALSE) OR (IsRemoved = TRUE AND RemovedBy IS NOT NULL AND RemovedAt IS NOT NULL)) ) NOT DEFERRABLE INITIALLY IMMEDIATE,
    CONSTRAINT DeviceCard_pk PRIMARY KEY (ID)
);

CREATE INDEX DeviceCard_idx_DeviceIndex on DeviceCard (Device_ID ASC);

CREATE INDEX DeviceCard_idx_SupplierIndex on DeviceCard (SupplierID ASC);

CREATE INDEX DeviceCard_idx_ServiceIndex on DeviceCard (ServiceID ASC);

CREATE INDEX DeviceCard_idx_MetConfIndex on DeviceCard (MetConfirmationID ASC);

-- Table: DeviceCard_DeviceOperation
CREATE TABLE DeviceCard_DeviceOperation (
    DeviceCard_ID bigint  NOT NULL,
    DeviceOperation_ID bigint  NOT NULL,
    CONSTRAINT DeviceCard_DeviceOperation_pk PRIMARY KEY (DeviceCard_ID,DeviceOperation_ID)
);

CREATE INDEX DeviceCard_DeviceOperation_idx_Card on DeviceCard_DeviceOperation (DeviceCard_ID ASC);

CREATE INDEX DeviceCard_DeviceOperation_idx_Op on DeviceCard_DeviceOperation (DeviceOperation_ID ASC);

-- Table: DeviceOperation
CREATE TABLE DeviceOperation (
    ID bigint  NOT NULL,
    UIElem bigint  NOT NULL,
    IsRequired boolean  NOT NULL DEFAULT false,
    Name varchar(100)  NOT NULL,
    Label varchar(50)  NOT NULL,
    Value decimal(5,5)  NULL,
    Unit varchar(10)  NULL,
    IsRemoved boolean  NOT NULL DEFAULT false,
    RemovedAt timestamp  NULL,
    RemovedBy varchar(36)  NULL,
    CreatedAt timestamp  NOT NULL DEFAULT current_timestamp,
    CreatedBy varchar(36)  NOT NULL,
    ModifiedAt timestamp  NOT NULL DEFAULT current_timestamp,
    ModifiedBy varchar(36)  NOT NULL,
    CONSTRAINT DeviceOperation_ValueUnit_CHK CHECK (((Value IS NULL AND Unit IS NULL)     OR (Value IS NOT NULL AND Unit IS NOT NULL))) NOT DEFERRABLE INITIALLY IMMEDIATE,
    CONSTRAINT DeviceOperation_IsRemoved_CHK CHECK (((IsRemoved = FALSE) OR (IsRemoved = TRUE AND RemovedBy IS NOT NULL AND RemovedAt IS NOT NULL)) ) NOT DEFERRABLE INITIALLY IMMEDIATE,
    CONSTRAINT DeviceOperation_pk PRIMARY KEY (ID)
);

CREATE INDEX DeviceOperation_UIElemIndex on DeviceOperation (UIElem ASC);

-- Table: Event
CREATE TABLE Event (
    ID int  NOT NULL,
    DeviceCard_ID bigint  NOT NULL,
    EventType int  NOT NULL,
    Description varchar(200)  NULL,
    IsRemoved boolean  NOT NULL DEFAULT false,
    RemovedAt timestamp  NULL,
    RemovedBy varchar(36)  NULL,
    CreatedAt timestamp  NOT NULL DEFAULT current_timestamp,
    CreatedBy varchar(36)  NOT NULL,
    ModifiedAt timestamp  NOT NULL DEFAULT current_timestamp,
    ModifiedBy varchar(36)  NOT NULL,
    CONSTRAINT Event_IsRemoved_CHK CHECK (((IsRemoved = FALSE)   OR  (IsRemoved = TRUE AND RemovedBy IS NOT NULL AND RemovedAt IS NOT NULL))) NOT DEFERRABLE INITIALLY IMMEDIATE,
    CONSTRAINT Event_pk PRIMARY KEY (ID)
);

CREATE INDEX Event_idx_DeviceCardIndex on Event (DeviceCard_ID ASC);

-- Table: Location
CREATE TABLE Location (
    ID bigint  NOT NULL,
    Name varchar(100)  NOT NULL,
    IsRemoved boolean  NOT NULL DEFAULT false,
    RemovedAt timestamp  NULL,
    RemovedBy varchar(36)  NULL,
    CreatedAt timestamp  NOT NULL DEFAULT current_timestamp,
    CreatedBy varchar(36)  NOT NULL,
    ModifiedAt timestamp  NOT NULL DEFAULT current_timestamp,
    ModifiedBy varchar(36)  NOT NULL,
    CONSTRAINT Location_IsRemoved_CHK CHECK (((IsRemoved = FALSE) OR (IsRemoved = TRUE AND RemovedBy IS NOT NULL AND RemovedAt IS NOT NULL)) ) NOT DEFERRABLE INITIALLY IMMEDIATE,
    CONSTRAINT Location_pk PRIMARY KEY (ID)
);

-- Table: MetConfirmation
CREATE TABLE MetConfirmation (
    ID bigint  NOT NULL,
    Name varchar(100)  NOT NULL,
    Lvl1 varchar(100)  NOT NULL,
    Lvl3 varchar(100)  NULL,
    Lvl4 varchar(100)  NULL,
    Lvl2 varchar(100)  NULL,
    IsRemoved boolean  NOT NULL DEFAULT false,
    RemovedAt timestamp  NULL,
    RemovedBy varchar(36)  NULL,
    CreatedAt timestamp  NOT NULL DEFAULT current_timestamp,
    CreatedBy varchar(36)  NOT NULL,
    ModifiedAt timestamp  NOT NULL DEFAULT current_timestamp,
    ModifiedBy varchar(36)  NOT NULL,
    CONSTRAINT MetConfirmation_IsRemoved_CHK CHECK (((IsRemoved = FALSE) OR (IsRemoved = TRUE AND RemovedBy IS NOT NULL AND RemovedAt IS NOT NULL)) ) NOT DEFERRABLE INITIALLY IMMEDIATE,
    CONSTRAINT MetConfirmation_pk PRIMARY KEY (ID)
);

-- Table: Service
CREATE TABLE Service (
    ID bigint  NOT NULL,
    Name varchar(100)  NOT NULL,
    Contact varchar(100)  NULL,
    IsRemoved boolean  NOT NULL DEFAULT false,
    RemovedAt timestamp  NULL,
    RemovedBy varchar(36)  NULL,
    CreatedAt timestamp  NOT NULL DEFAULT current_timestamp,
    CreatedBy varchar(36)  NOT NULL,
    ModifiedAt timestamp  NOT NULL DEFAULT current_timestamp,
    ModifiedBy varchar(36)  NOT NULL,
    CONSTRAINT Service_IsRemoved_CHK CHECK (((IsRemoved = FALSE) OR (IsRemoved = TRUE AND RemovedBy IS NOT NULL AND RemovedAt IS NOT NULL)) ) NOT DEFERRABLE INITIALLY IMMEDIATE,
    CONSTRAINT Service_pk PRIMARY KEY (ID)
);

-- Table: Supplier
CREATE TABLE Supplier (
    ID bigint  NOT NULL,
    Name varchar(100)  NOT NULL,
    Contact varchar(100)  NULL,
    IsRemoved boolean  NOT NULL DEFAULT false,
    RemovedAt timestamp  NULL,
    RemovedBy varchar(36)  NULL,
    CreatedAt timestamp  NOT NULL DEFAULT current_timestamp,
    CreatedBy varchar(36)  NOT NULL,
    ModifiedAt timestamp  NOT NULL DEFAULT current_timestamp,
    ModifiedBy varchar(36)  NOT NULL,
    CONSTRAINT Supplier_IsRemoved_CHK CHECK (((IsRemoved = FALSE) OR (IsRemoved = TRUE AND RemovedBy IS NOT NULL AND RemovedAt IS NOT NULL))) NOT DEFERRABLE INITIALLY IMMEDIATE,
    CONSTRAINT Supplier_pk PRIMARY KEY (ID)
);

-- Table: TempProdTempStep
CREATE TABLE TempProdTempStep (
    TemplateProductionID bigint  NOT NULL,
    TemplateStepID bigint  NOT NULL,
    "Order" int  NOT NULL,
    CONSTRAINT TempProdTempStep_pk PRIMARY KEY (TemplateProductionID,TemplateStepID)
);

CREATE INDEX TempProdTempStep_Order_idx on TempProdTempStep ("Order" ASC);

CREATE INDEX TempProdTempStep_idx_ProdIndex on TempProdTempStep (TemplateProductionID ASC);

CREATE INDEX TempProdTempStep_idx_StepIndex on TempProdTempStep (TemplateStepID ASC);

-- Table: TempStepOperations
CREATE TABLE TempStepOperations (
    TemplateStepID bigint  NOT NULL,
    "Order" int  NOT NULL,
    TemplateOperationID bigint  NULL,
    DeviceOperationID bigint  NULL,
    IsDeviceOp boolean  NOT NULL,
    CONSTRAINT TempStepOperations_OnlyOneOp_CHK CHECK (((TemplateOperationID IS NOT NULL AND DeviceOperationID IS NULL AND IsDeviceOp = FALSE) OR (TemplateOperationID IS NULL AND DeviceOperationID IS NOT NULL AND IsDeviceOp = TRUE))) NOT DEFERRABLE INITIALLY IMMEDIATE,
    CONSTRAINT TempStepOperations_pk PRIMARY KEY (TemplateStepID,"Order")
);

CREATE UNIQUE INDEX TempStepOperations_StepOp_Unique_idx on TempStepOperations (TemplateStepID ASC,TemplateOperationID ASC)
    WHERE TemplateOperationID IS NOT NULL;

CREATE UNIQUE INDEX TempStepOperations_StepDevOp_Unique_idx on TempStepOperations (TemplateStepID ASC,DeviceOperationID ASC)
    WHERE DeviceOperationID IS NOT NULL;

CREATE INDEX TempStepOperations_Step_idx on TempStepOperations (TemplateStepID ASC);

CREATE INDEX TempStepOperations_Op_idx on TempStepOperations (TemplateOperationID ASC);

CREATE INDEX TempStepOperations_DevOp_idx on TempStepOperations (DeviceOperationID ASC);

CREATE INDEX TempStepOperations_IsDeviceOp_idx on TempStepOperations (IsDeviceOp ASC);

-- Table: TemplateOperation
CREATE TABLE TemplateOperation (
    ID bigint  NOT NULL,
    UIElem bigint  NOT NULL,
    Label varchar(50)  NOT NULL,
    TempValue decimal(5,5)  NULL,
    TempUnit varchar(10)  NULL,
    IsRemoved boolean  NOT NULL DEFAULT false,
    RemovedBy varchar(36)  NULL,
    RemovedAt timestamp  NULL,
    CreatedBy varchar(36)  NOT NULL,
    CreatedAt timestamp  NOT NULL DEFAULT current_timestamp,
    ModifiedBy varchar(36)  NOT NULL,
    ModifiedAt timestamp  NOT NULL DEFAULT current_timestamp,
    CONSTRAINT TemplateOperation_ValueUnit_CHK CHECK (((TempValue IS NULL AND TempUnit IS NULL)     OR (TempValue IS NOT NULL AND TempUnit IS NOT NULL))) NOT DEFERRABLE INITIALLY IMMEDIATE,
    CONSTRAINT TemplateOperation_IsRemoved_CHK CHECK (((IsRemoved = FALSE) OR (IsRemoved = TRUE AND RemovedBy IS NOT NULL AND RemovedAt IS NOT NULL)) ) NOT DEFERRABLE INITIALLY IMMEDIATE,
    CONSTRAINT TemplateOperation_pk PRIMARY KEY (ID)
);

CREATE INDEX TemplateOperation_UIElemIndex on TemplateOperation (UIElem ASC);

-- Table: TemplateProduction
CREATE TABLE TemplateProduction (
    ID bigint  NOT NULL,
    Name varchar(100)  NOT NULL,
    BaseUnit varchar(10)  NOT NULL,
    BaseValue decimal(5,5)  NOT NULL,
    IsRemoved boolean  NOT NULL DEFAULT false,
    RemovedBy varchar(36)  NULL,
    RemovedAt timestamp  NULL,
    CreatedBy varchar(36)  NOT NULL,
    CreatedAt timestamp  NOT NULL DEFAULT current_timestamp,
    ModifiedBy varchar(36)  NOT NULL,
    ModifiedAt timestamp  NOT NULL DEFAULT current_timestamp,
    CONSTRAINT TemplateProduction_IsRemoved_CHK CHECK (((IsRemoved = FALSE) OR (IsRemoved = TRUE AND RemovedBy IS NOT NULL AND RemovedAt IS NOT NULL))) NOT DEFERRABLE INITIALLY IMMEDIATE,
    CONSTRAINT TemplateProduction_pk PRIMARY KEY (ID)
);

-- Table: TemplateStep
CREATE TABLE TemplateStep (
    ID bigint  NOT NULL,
    Name varchar(100)  NOT NULL,
    IsRemoved boolean  NOT NULL DEFAULT false,
    RemovedBy varchar(36)  NULL,
    RemovedAt timestamp  NULL,
    CreatedBy varchar(36)  NOT NULL,
    CreatedAt timestamp  NOT NULL DEFAULT current_timestamp,
    ModifiedBy varchar(36)  NOT NULL,
    ModifiedAt timestamp  NOT NULL DEFAULT current_timestamp,
    CONSTRAINT TemplateStep_IsRemoved_CHK CHECK (((IsRemoved = FALSE) OR (IsRemoved = TRUE AND RemovedBy IS NOT NULL AND RemovedAt IS NOT NULL))) NOT DEFERRABLE INITIALLY IMMEDIATE,
    CONSTRAINT TemplateStep_pk PRIMARY KEY (ID)
);

-- Table: TemplateUIElem
CREATE TABLE TemplateUIElem (
    ID bigint  NOT NULL,
    Name varchar(20)  NOT NULL,
    ElemType int  NOT NULL,
    CONSTRAINT TemplateUIElem_pk PRIMARY KEY (ID)
);

-- Table: WorkOperation
CREATE TABLE WorkOperation (
    ID bigint  NOT NULL,
    WorkStep_ID bigint  NOT NULL,
    Name varchar(100)  NOT NULL,
    Label varchar(50)  NOT NULL,
    Value decimal(5,5)  NULL,
    Unit varchar(10)  NULL,
    IsDeviceOp boolean  NOT NULL DEFAULT false,
    BoundDevice bigint  NULL,
    "Order" int  NOT NULL,
    WorkingUser varchar(36)  NULL,
    IsDone boolean  NOT NULL DEFAULT false,
    DoneBy varchar(36)  NULL,
    DoneAt timestamp  NULL,
    ModifiedAt timestamp  NOT NULL DEFAULT current_timestamp,
    ModifiedBy varchar(36)  NOT NULL,
    CONSTRAINT WorkOperation_BoundDevice_CHK CHECK (( ( IsDeviceOp = FALSE OR BoundDevice IS NOT NULL ) )) NOT DEFERRABLE INITIALLY IMMEDIATE,
    CONSTRAINT WorkOperation_ValueUnit_CHK CHECK (((Value IS NULL AND Unit IS NULL)     OR (Value IS NOT NULL AND Unit IS NOT NULL))) NOT DEFERRABLE INITIALLY IMMEDIATE,
    CONSTRAINT WorkOperation_IsDone_CHK CHECK ((     (IsDone = FALSE)     OR     (IsDone = TRUE AND DoneBy IS NOT NULL AND DoneAt IS NOT NULL) )) NOT DEFERRABLE INITIALLY IMMEDIATE,
    CONSTRAINT WorkOperation_pk PRIMARY KEY (ID)
);

CREATE INDEX WorkOperation_idx_WorkStepIndex on WorkOperation (WorkStep_ID ASC);

CREATE INDEX WorkOperation_idx_StepOrderIndex on WorkOperation (WorkStep_ID ASC,"Order" ASC);

CREATE INDEX WorkOperation_idx_BoundDevice on WorkOperation (BoundDevice ASC);

-- Table: WorkProduction
CREATE TABLE WorkProduction (
    ID bigint  NOT NULL,
    Name varchar(100)  NOT NULL,
    Value decimal(5,5)  NOT NULL,
    Unit varchar(10)  NOT NULL,
    AssignedUser varchar(36)  NOT NULL,
    IsDisabled boolean  NOT NULL DEFAULT false,
    DisabledBy varchar(36)  NULL,
    DisabledAt timestamp  NULL,
    IsRemoved boolean  NOT NULL DEFAULT false,
    RemovedBy varchar(36)  NULL,
    RemovedAt timestamp  NULL,
    CreatedBy varchar(36)  NOT NULL,
    CreatedAt timestamp  NOT NULL DEFAULT current_timestamp,
    IsDone boolean  NOT NULL DEFAULT false,
    DoneBy varchar(36)  NULL,
    DoneAt timestamp  NULL,
    ModifiedAt timestamp  NOT NULL DEFAULT current_timestamp,
    ModifiedBy varchar(36)  NOT NULL,
    CONSTRAINT WorkProduction_IsRemoved_CHK CHECK (((IsRemoved = FALSE) OR (IsRemoved = TRUE AND RemovedBy IS NOT NULL AND RemovedAt IS NOT NULL))) NOT DEFERRABLE INITIALLY IMMEDIATE,
    CONSTRAINT WorkProduction_IsDisabled_CHK CHECK ((     (IsDisabled = FALSE)     OR     (IsDisabled = TRUE AND DisabledBy IS NOT NULL AND DisabledAt IS NOT NULL) )) NOT DEFERRABLE INITIALLY IMMEDIATE,
    CONSTRAINT WorkProduction_IsDone_CHK CHECK ((     (IsDone = FALSE)     OR     (IsDone = TRUE AND DoneBy IS NOT NULL AND DoneAt IS NOT NULL) )) NOT DEFERRABLE INITIALLY IMMEDIATE,
    CONSTRAINT WorkProduction_pk PRIMARY KEY (ID)
);

-- Table: WorkStep
CREATE TABLE WorkStep (
    ID bigint  NOT NULL,
    WorkProduction_ID bigint  NOT NULL,
    Name varchar(100)  NOT NULL,
    "Order" int  NOT NULL,
    IsDone boolean  NOT NULL DEFAULT false,
    DoneBy varchar(36)  NULL,
    DoneAt timestamp  NULL,
    ModifiedAt timestamp  NOT NULL DEFAULT current_timestamp,
    ModifiedBy varchar(36)  NOT NULL,
    CONSTRAINT WorkStep_IsDone_CHK CHECK ((     (IsDone = FALSE)     OR     (IsDone = TRUE AND DoneBy IS NOT NULL AND DoneAt IS NOT NULL) )) NOT DEFERRABLE INITIALLY IMMEDIATE,
    CONSTRAINT WorkStep_pk PRIMARY KEY (ID)
);

CREATE INDEX WorkStep_idx_WorkProdIndex on WorkStep (WorkProduction_ID ASC);

CREATE INDEX WorkStep_idx_ProdOrderIndex on WorkStep (WorkProduction_ID ASC,"Order" ASC);

-- Table: WorkUIElem
CREATE TABLE WorkUIElem (
    ID bigint  NOT NULL,
    WorkOperation_ID bigint  NOT NULL,
    Type int  NOT NULL,
    Data varchar(200)  NULL,
    IsDone boolean  NOT NULL DEFAULT false,
    DoneBy varchar(36)  NULL,
    DoneAt timestamp  NULL,
    ModifiedAt timestamp  NOT NULL DEFAULT current_timestamp,
    ModifiedBy varchar(36)  NOT NULL,
    CONSTRAINT WorkUIElem_IsDone_CHK CHECK ((     (IsDone = FALSE)     OR     (IsDone = TRUE AND DoneBy IS NOT NULL AND DoneAt IS NOT NULL) )) NOT DEFERRABLE INITIALLY IMMEDIATE,
    CONSTRAINT WorkUIElem_pk PRIMARY KEY (ID)
);

CREATE INDEX WorkUIElem_idx_WorkOperationIndex on WorkUIElem (WorkOperation_ID ASC);

-- foreign keys
-- Reference: DCDO_DeviceCard_FK (table: DeviceCard_DeviceOperation)
ALTER TABLE DeviceCard_DeviceOperation ADD CONSTRAINT DCDO_DeviceCard_FK
    FOREIGN KEY (DeviceCard_ID)
    REFERENCES DeviceCard (ID)
    ON DELETE  CASCADE
    NOT DEFERRABLE
    INITIALLY IMMEDIATE
;

-- Reference: DCDO_DeviceOperation_FK (table: DeviceCard_DeviceOperation)
ALTER TABLE DeviceCard_DeviceOperation ADD CONSTRAINT DCDO_DeviceOperation_FK
    FOREIGN KEY (DeviceOperation_ID)
    REFERENCES DeviceOperation (ID)
    ON DELETE  CASCADE
    NOT DEFERRABLE
    INITIALLY IMMEDIATE
;

-- Reference: DeviceCard_Device_FK (table: DeviceCard)
ALTER TABLE DeviceCard ADD CONSTRAINT DeviceCard_Device_FK
    FOREIGN KEY (Device_ID)
    REFERENCES Device (ID)
    ON DELETE  CASCADE
    NOT DEFERRABLE
    INITIALLY IMMEDIATE
;

-- Reference: DeviceCard_MetConfirmation_FK (table: DeviceCard)
ALTER TABLE DeviceCard ADD CONSTRAINT DeviceCard_MetConfirmation_FK
    FOREIGN KEY (MetConfirmationID)
    REFERENCES MetConfirmation (ID)
    ON DELETE  RESTRICT
    NOT DEFERRABLE
    INITIALLY IMMEDIATE
;

-- Reference: DeviceCard_Service_FK (table: DeviceCard)
ALTER TABLE DeviceCard ADD CONSTRAINT DeviceCard_Service_FK
    FOREIGN KEY (ServiceID)
    REFERENCES Service (ID)
    ON DELETE  RESTRICT
    NOT DEFERRABLE
    INITIALLY IMMEDIATE
;

-- Reference: DeviceCard_Supplier_FK (table: DeviceCard)
ALTER TABLE DeviceCard ADD CONSTRAINT DeviceCard_Supplier_FK
    FOREIGN KEY (SupplierID)
    REFERENCES Supplier (ID)
    ON DELETE  RESTRICT
    NOT DEFERRABLE
    INITIALLY IMMEDIATE
;

-- Reference: Device_Location_FK (table: Device)
ALTER TABLE Device ADD CONSTRAINT Device_Location_FK
    FOREIGN KEY (LocationID)
    REFERENCES Location (ID)
    ON DELETE  RESTRICT
    NOT DEFERRABLE
    INITIALLY IMMEDIATE
;

-- Reference: Device_WorkOperation (table: WorkOperation)
ALTER TABLE WorkOperation ADD CONSTRAINT Device_WorkOperation
    FOREIGN KEY (BoundDevice)
    REFERENCES Device (ID)
    NOT DEFERRABLE
    INITIALLY IMMEDIATE
;

-- Reference: Event_DeviceCard_FK (table: Event)
ALTER TABLE Event ADD CONSTRAINT Event_DeviceCard_FK
    FOREIGN KEY (DeviceCard_ID)
    REFERENCES DeviceCard (ID)
    ON DELETE  CASCADE
    NOT DEFERRABLE
    INITIALLY IMMEDIATE
;

-- Reference: TPTS_TemplateProduction_FK (table: TempProdTempStep)
ALTER TABLE TempProdTempStep ADD CONSTRAINT TPTS_TemplateProduction_FK
    FOREIGN KEY (TemplateProductionID)
    REFERENCES TemplateProduction (ID)
    ON DELETE  CASCADE
    NOT DEFERRABLE
    INITIALLY IMMEDIATE
;

-- Reference: TPTS_TemplateStep_FK (table: TempProdTempStep)
ALTER TABLE TempProdTempStep ADD CONSTRAINT TPTS_TemplateStep_FK
    FOREIGN KEY (TemplateStepID)
    REFERENCES TemplateStep (ID)
    ON DELETE  CASCADE
    NOT DEFERRABLE
    INITIALLY IMMEDIATE
;

-- Reference: TSO_DeviceOperation_FK (table: TempStepOperations)
ALTER TABLE TempStepOperations ADD CONSTRAINT TSO_DeviceOperation_FK
    FOREIGN KEY (DeviceOperationID)
    REFERENCES DeviceOperation (ID)
    ON DELETE  CASCADE
    NOT DEFERRABLE
    INITIALLY IMMEDIATE
;

-- Reference: TSO_TemplateOperation_FK (table: TempStepOperations)
ALTER TABLE TempStepOperations ADD CONSTRAINT TSO_TemplateOperation_FK
    FOREIGN KEY (TemplateOperationID)
    REFERENCES TemplateOperation (ID)
    ON DELETE  CASCADE
    NOT DEFERRABLE
    INITIALLY IMMEDIATE
;

-- Reference: TSO_TemplateStep_FK (table: TempStepOperations)
ALTER TABLE TempStepOperations ADD CONSTRAINT TSO_TemplateStep_FK
    FOREIGN KEY (TemplateStepID)
    REFERENCES TemplateStep (ID)
    ON DELETE  CASCADE
    NOT DEFERRABLE
    INITIALLY IMMEDIATE
;

-- Reference: TemplateUIElem_DeviceOperation (table: DeviceOperation)
ALTER TABLE DeviceOperation ADD CONSTRAINT TemplateUIElem_DeviceOperation
    FOREIGN KEY (UIElem)
    REFERENCES TemplateUIElem (ID)
    NOT DEFERRABLE
    INITIALLY IMMEDIATE
;

-- Reference: TemplateUIElem_TemplateOperation (table: TemplateOperation)
ALTER TABLE TemplateOperation ADD CONSTRAINT TemplateUIElem_TemplateOperation
    FOREIGN KEY (UIElem)
    REFERENCES TemplateUIElem (ID)
    NOT DEFERRABLE
    INITIALLY IMMEDIATE
;

-- Reference: WorkOperation_WorkStep_FK (table: WorkOperation)
ALTER TABLE WorkOperation ADD CONSTRAINT WorkOperation_WorkStep_FK
    FOREIGN KEY (WorkStep_ID)
    REFERENCES WorkStep (ID)
    ON DELETE  CASCADE
    NOT DEFERRABLE
    INITIALLY IMMEDIATE
;

-- Reference: WorkStep_WorkProduction_FK (table: WorkStep)
ALTER TABLE WorkStep ADD CONSTRAINT WorkStep_WorkProduction_FK
    FOREIGN KEY (WorkProduction_ID)
    REFERENCES WorkProduction (ID)
    ON DELETE  CASCADE
    NOT DEFERRABLE
    INITIALLY IMMEDIATE
;

-- Reference: WorkUIElem_WorkOperation_FK (table: WorkUIElem)
ALTER TABLE WorkUIElem ADD CONSTRAINT WorkUIElem_WorkOperation_FK
    FOREIGN KEY (WorkOperation_ID)
    REFERENCES WorkOperation (ID)
    ON DELETE  CASCADE
    NOT DEFERRABLE
    INITIALLY IMMEDIATE
;

-- sequences
-- Sequence: DeviceBase_seq
CREATE SEQUENCE DeviceBase_seq
      INCREMENT BY 1
      NO MINVALUE
      NO MAXVALUE
      START WITH 1
      NO CYCLE
      AS bigint
;

-- Sequence: TemplateBase_seq
CREATE SEQUENCE TemplateBase_seq
      INCREMENT BY 1
      NO MINVALUE
      NO MAXVALUE
      START WITH 1
      NO CYCLE
      AS bigint
;

-- Sequence: WorkBase_seq
CREATE SEQUENCE WorkBase_seq
      INCREMENT BY 1
      NO MINVALUE
      NO MAXVALUE
      START WITH 1
      NO CYCLE
      AS bigint
;

-- End of file.
````

```

```

```

```

```

```
