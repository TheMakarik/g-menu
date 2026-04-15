local TEST_SOLUTION_DIRECTORY = "tests/GMenu.Testing";
local CANNOT_FOUND_EXIT_CODE = 127;

if os.execute("dotnet --version > /dev/null 2>&1")/ 256  == CANNOT_FOUND_EXIT_CODE then
    print("dotnet is not found, please install dotnet to run the tests");
end 

os.exit(os.execute("dotnet test " .. TEST_SOLUTION_DIRECTORY) / 256);