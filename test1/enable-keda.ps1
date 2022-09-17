$rg = "k8-test1";
$nm = "aks-test1";
az aks update --resource-group $rg --name $nm --enable-keda

az aks get-credentials --resource-group $rg --name $nm

az aks show -g $rg --name $nm --query "workloadAutoScalerProfile.keda.enabled"

kubectl get pods -n kube-system

kubectl get crd/scaledobjects.keda.sh -o yaml


