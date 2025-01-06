#!/bin/bash

# Start the API
dotnet run --project TodoBackend/TodoBackend.csproj &
API_PID=$!

# Wait for API to start
sleep 5

# Run Postman tests
newman run TodoAPI.postman_collection.json \
  -e TodoAPI.postman_environment.json \
  -r cli,htmlextra \
  --reporter-htmlextra-export testResults/htmlreport.html

# Store test result
TEST_RESULT=$?

# Kill the API
kill $API_PID

# Exit with test result
exit $TEST_RESULT 