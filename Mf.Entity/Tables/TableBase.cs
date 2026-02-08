using System.ComponentModel.DataAnnotations;

namespace Mf.Entity.Tables;

public abstract class TableBase<T>:ITable
{
    [Key]
    public T? Id { get; set; }

    private List<TableBase<T>>? _seedData = null;
}

public  abstract class Tablebase : TableBase<long>
{
    
    public Tablebase()
    {
        
    }
}

