# _Radar Target_RECEIVER (레이더 타겟 데이터 수집기)_

## 설명
 - UDP 유니캐스트로 수신되는 레이더 타겟 정보를 카프카 클러스터에 저장하는 프로듀서
 - `1.0.4` : protobuf 에 타임스탭프 추가, status 정보 추가.

 - `1.0.5` : 콘솔창에 파싱한 데이터 출력

 - `1.0.6` : 수신된 데이터 값 로그 남기기.


 #### 파라미터
  `[0]` : UDP 클라이언트 포트

#### 환경변수

`KAFKA` : 카프카 클러스터 주소정보

### 도커 이미지 생성
```
docker build -f . --force-rm -t target_receiver:1.0.6 --label="CloutVTS" .

```
### 도커 컨테이너 생성
```
docker run -dt --name target_receiver103 \
      -e KAFKA=$KAFKA\
      --log-opt max-file=5 \
      --log-opt max-size=10m \
      --network host \
      --restart always \
      -v /etc/localtime:/etc/localtime \
      -v /log/target:/logs \
      target_receiver:1.0.6 10302
```

### 도커 파일 압축
```
docker save -o targetreceiver106.tar target_receiver:1.0.6
```