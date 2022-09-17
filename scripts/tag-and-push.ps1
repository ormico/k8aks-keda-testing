<#
.SYNOPSIS
Push and Tag ormico/akskedademo containers
.DESCRIPTION    
Push and Tag ormico/akskedademo containers
.PARAMETER Version
Version string in the format 1.2.3.4
.EXAMPLE
.\tag-and-push -Version "1.2.3.4"
#>

param
(
    [Parameter(Mandatory=$true)][string]$Version
)

try
{
    # tag
    #docker tag ormico/akskedademo.consoleworker1:latest ormicodemo.azurecr.io/ormico/akskedademo.consoleworker1:$Version;
    #docker tag ormico/akskedademo.basicreaderworker:latest ormicodemo.azurecr.io/ormico/akskedademo.basicreaderworker:$Version;
    #docker tag ormico/akskedademo.basicwriterworker:latest ormicodemo.azurecr.io/ormico/akskedademo.basicwriterworker:$Version;
    docker tag ormico/akskedademo.consoleworker1:latest ormicodemo.azurecr.io/akskedademo.consoleworker1:$Version;
    docker tag ormico/akskedademo.basicreaderworker:latest ormicodemo.azurecr.io/akskedademo.basicreaderworker:$Version;
    docker tag ormico/akskedademo.basicwriterworker:latest ormicodemo.azurecr.io/akskedademo.basicwriterworker:$Version;

    # push
    #docker push ormicodemo.azurecr.io/ormico/akskedademo.consoleworker1:$Version;
    #docker push ormicodemo.azurecr.io/ormico/akskedademo.basicreaderworker:$Version;
    #docker push ormicodemo.azurecr.io/ormico/akskedademo.basicwriterworker:$Version;
    docker push ormicodemo.azurecr.io/akskedademo.consoleworker1:$Version;
    docker push ormicodemo.azurecr.io/akskedademo.basicreaderworker:$Version;
    docker push ormicodemo.azurecr.io/akskedademo.basicwriterworker:$Version;
    
    # cleanup
    #docker rmi ormicodemo.azurecr.io/ormico/akskedademo.consoleworker1:$Version;
    #docker rmi ormicodemo.azurecr.io/ormico/akskedademo.basicreaderworker:$Version;
    #docker rmi ormicodemo.azurecr.io/ormico/akskedademo.basicwriterworker:$Version;
    docker rmi ormicodemo.azurecr.io/akskedademo.consoleworker1:$Version;
    docker rmi ormicodemo.azurecr.io/akskedademo.basicreaderworker:$Version;
    docker rmi ormicodemo.azurecr.io/akskedademo.basicwriterworker:$Version;
}
catch
{
    Write-Host "Error: $_";
    exit -1;
}
exit 0;
