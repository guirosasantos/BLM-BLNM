namespace Lib;

public sealed record Instance(
    int NumberOfMachines, //M
    double R
)
{
    public Machine[] Machines { get; set; } = new Machine[NumberOfMachines];
};