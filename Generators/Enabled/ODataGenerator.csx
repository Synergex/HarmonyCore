
public class CustomODataGenerator : ODataGenerator
{
    public override void ApplyDefaults(Solution solution)
    {

    }

    public override List<string> ValidateSolution(Solution solution)
    {
        return new List<string>();
    }

    public override List<CodeGenTask> GenerateTasks(Solution solution)
    {
        var baseTasks = base.GenerateTasks(solution);
        baseTasks.Add(StructureTaskHelper(solution, "Generate controller properties partial classes", solution.ControllersNamespace, solution.ControllersFolder, false, nameof(ODataGenerator), new string[] { "ODataControllerPropertyEndpoints" }, null));
		return baseTasks;
	}
}

new CustomODataGenerator()