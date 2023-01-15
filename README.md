# EazTrialRemover
Works for the latest eaz version 👍
# Manual way if it doesn't work using the tool
1. Open file in dnspy (https://github.com/dnSpyEx/dnSpy)
2. Go to entrypoint
3. There should be a check, that function should be the trial check will look something like this
```cs
if (!\u0003.\u0002())
{
  return;
}
```
4. Click the function it should redirect you to where it was declared
5. Right click it and click `Edit IL Instructions` and select the call instruction right click and do `Nop instruction`
6. Save the file should work 👍
