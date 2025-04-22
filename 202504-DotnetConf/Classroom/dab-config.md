## dab-config.json
<p>&nbsp;</p>

```mermaid
stateDiagram-v2
direction LR

  classDef empty fill:none,stroke:none
  classDef table stroke:black;
  classDef view stroke:black;
  classDef proc stroke:black;
  classDef phantom stroke:gray,stroke-dasharray:5 5;

  class NoTables empty
  class NoViews empty
  class NoProcs empty

  class Attendance table
  class Classes table
  class Students table
  state Tables {
    Attendance --> Classes
    Attendance --> Students
    Classes --> Attendance
    Classes --> Students
    Students --> Classes
    Students --> Attendance
  }
  state Views {
    NoViews
  }
  state Procedures {
    NoProcs
  }
```

### Tables
|Entity|Source|Relationships
|-|-|-
|Attendance|dbo.Attendance|Classes, Students
|Classes|dbo.Classes|Attendance, Students
|Students|dbo.Students|Classes, Attendance

### Views
> None

### Stored Procedures
> None

