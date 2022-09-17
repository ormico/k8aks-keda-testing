# https://github.com/kedacore/sample-dotnet-worker-servicebus-queue/blob/main/connection-string-scenario.md
az servicebus queue authorization-rule keys list -g k8-test1 --namespace-name sb-k8test1 --queue-name q1 -n BasicSendReceiveQuickStart

# in bash 
# echo -n "<connection string>" | base64
