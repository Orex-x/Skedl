namespace Skedl.AuthService.Services.CodeGeneration;

public class CodeGenerator : ICodeGenerator
{
    private readonly string _baseLine = "1234567890";
    
    public string Generation(int length)
    {
        var code = string.Empty;

        var random = new Random(); 
        
        for (int i = 0; i < length; i++)
        {
            code += _baseLine[random.Next(0, _baseLine.Length)];
        }

        return code;
    }
    
}