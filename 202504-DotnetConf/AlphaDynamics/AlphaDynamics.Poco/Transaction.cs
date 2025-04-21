namespace AlphaDynamics.Poco;

public class Transaction
{
    public int Id { get; set; }
    public DateTime Date { get; set; }

    public int CrewId { get; set; }
    public Crew Crew { get; set; } = default!;

    public int EquipmentId { get; set; }
    public Equipment Equipment { get; set; } = default!;

    public int OperationId { get; set; }
    public Operation Operation { get; set; } = default!;
}