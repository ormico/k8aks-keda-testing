$version = '1.0.0.1';

# tag
docker tag ormico/akskedademo.consoleworker1:latest ormicodemo.azurecr.io/ormico/akskedademo.consoleworker1:$version

docker tag ormico/akskedademo.basicreaderworker:latest ormicodemo.azurecr.io/ormico/akskedademo.basicreaderworker:$version

docker tag ormico/akskedademo.basicwriterworker:latest ormicodemo.azurecr.io/ormico/akskedademo.basicwriterworker:$version

# push
docker push ormicodemo.azurecr.io/ormico/akskedademo.consoleworker1:$version
docker push ormicodemo.azurecr.io/ormico/akskedademo.basicreaderworker:$version
docker push ormicodemo.azurecr.io/ormico/akskedademo.basicwriterworker:$version

# cleanup
docker rmi ormicodemo.azurecr.io/ormico/akskedademo.consoleworker1:$version

docker rmi ormicodemo.azurecr.io/ormico/akskedademo.basicreaderworker:$version

docker rmi ormicodemo.azurecr.io/ormico/akskedademo.basicwriterworker:$version
