 
for($i = 0; $i -lt 1000; $i++)
{
    Write-Output "Iteration $($i +1 )"
    
    .\OrmBenchmark.Console.exe | Out-Null
    Write-Output "Iteration end $($i +1 )"
    Start-Sleep -Seconds 2
}
 
 
 