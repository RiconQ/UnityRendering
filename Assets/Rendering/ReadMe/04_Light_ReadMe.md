Directional Light는 씬 전체에 동일한 방향으로 빛을 비춤

오브젝트가 빛을 어떻게 반사하는지 계산하는 모델이 필요함 -> Lambert Model 사용
-> 표면이 빛을 정면으로 받을수록 가장 밝고, 비스듬히 받을수록 어두워짐

Normal 벡터로 표면의 방향 계산

[Vertex Shader]
Model의 정점 위치를 Object -> World -> Clip Space로 변환
Normal Vector를 Object->World 공간으로 변환 (기존 Normal은 Object 공간이고 Light는 World 공간이므로 기준이 다름)

[Fragment Shader]
Vertex Shader로 부터 받은 World Space Normal값을 World Space Light와 내적함
해당 내적 결과에 Light Color를 곱하여 빛의 세기를 계산하고 이를 Albedo와 곱하여 최종 색상을 반환함

[Specular]
유광 재질을 표현하기 위한 정반사광. LgithDir, NormalDir, ViewDir을 사용하여 계산

----------------------------------------------------------------
[Phong]
빛 방향과 노멀 방향을 이용하여 반사 벡터 계산
이 반사 벡터와 시점 벡터를 내적하여 1에 가까울수록 가장 밝은 하이라이트 생성

[Blinn Phong]
Phong 모델의 계산을 최적화 한 버전
빛 방향과 시점 방향의 중간 벡터를 계산(두 벡터의 중간 지점을 가리키는 벡터)
중간 벡터와 노멀 방향을 내적하여 1에 가까울수록 가장 밝은 하이라이트 생성

[Shininess]
Phong, Blinn Phong에서 계산된 내적 결과를 거듭제곱하여 계산(Pow(dotResult, Shininess))