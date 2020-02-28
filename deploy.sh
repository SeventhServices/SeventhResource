#!/bin/sh

	if test -n "$1" && test -n "$2"; then
		echo "container name : $1"
		echo "image name : $2"; 
	else 
		echo "invalid argument...";
		exit 1;
	fi

	containers=$(docker ps -q --filter name=$1)
	if test -n "$containers"; then
		echo "found container : $containers"
		docker stop $(docker ps -q --filter name=$1) >> /dev/null 2>&1
		rc=$?
			if [[ $rc != 0 ]];
			  then
				echo 'failed to stop container...'
				exit $rc;
			fi
		echo "toped container : $containers"
	fi
	   
	containers_stop=$(docker ps -q -a --filter name=$1)
	if test -n "$containers_stop"; then
		docker rm $(docker ps -q -a --filter name=$1) >> /dev/null 2>&1
		rc=$?
			if [[ $rc != 0 ]];
			then
				echo 'failed to remove container...'
				exit $rc;
			fi
		echo "removed container : $containers_stop"
	fi
	
	echo "start pull image : $2"
	docker pull $2 >> /dev/null 2>&1
	rc=$?
	if [[ $rc != 0 ]]; then
		echo 'failed to pull container...'
		exit $rc;
	fi
	echo "pulled image : $2"
	
	danglings=$(docker images -f "dangling=true" -q)
	if test -n "$danglings"; then
		docker rmi $(docker images -f "dangling=true" -q) >> /dev/null 2>&1
		rc=$?
			if [[ $rc != 0 ]];
			then
				echo 'failed to remove danglings container...'
				exit $rc;
			fi
		echo "removed old image : $danglings"
	fi
	   
	containers_new=$(docker run -d --restart=always \
		-v /data/sites/resource.t7s.sagilio.net/www/wwwroot:/app/wwwroot \
	    -p 5000:80 --name $1 $2 ) >> /dev/null 2>&1
	rc=$?
	if [[ $rc != 0 ]]; then
		echo 'failed to run container...'
		exit $rc;
	fi
	echo "start new container : $containers_new"
