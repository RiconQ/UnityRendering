# Matrices

## 1. 기본 변환 (Transformations)

3D 모델의 정점(Vertex)은 위치(Position), 회전(Rotation), 크기(Scale) 변환을 거쳐 3D 공간에서의 최종 상태가 결정됩니다.

### 1.1. 위치 (Position / Translation)

- **개념:** 물체를 특정 방향으로 '이동'시킵니다.
- **계산:** 현재 정점의 위치 벡터(`Point.pos`)에 이동할 거리와 방향을 나타내는 변위 벡터(`Position`)를 더합니다.
- **수학식:** 벡터 덧셈을 사용합니다.
    
    $$
    p'_x = p_x + t_x \\
    p'_y = p_y + t_y \\
    p'_z = p_z + t_z
    $$
    
    - $p$ : 원래 정점 위치 ( $[p_x, p_y, p_z]$ )
    - $t$ : 이동 벡터 ( $[t_x, t_y, t_z]$ )
    - $p'$ : 이동 후 정점 위치

### 1.2. 크기 (Scale)

- **개념:** 물체를 특정 축을 기준으로 '확대' 또는 '축소'시킵니다.
- **계산:** 현재 정점의 위치 벡터(`Point.pos`)의 각 성분(x, y, z)에 스케일 벡터(`scale`)의 각 성분을 곱합니다.
- **수학식:** 성분별 곱셈(Component-wise multiplication)을 사용합니다.
    
    $$
    p'_x = p_x \cdot s_x \\
    p'_y = p_y \cdot s_y \\
    p'_z = p_z \cdot s_z
    $$
    
    - $p$ : 원래 정점 위치 ( $[p_x, p_y, p_z]$ )
    - $s$ : 스케일 벡터 ( $[s_x, s_y, s_z]$ )
    - $p'$ : 크기 변환 후 정점 위치

### 1.3. 회전 (Rotation)

- **개념:** 물체를 특정 축을 기준으로 '회전'시킵니다.
- **계산:** 삼각함수(Cos, Sin)를 사용하여 회전 각도에 따른 변환을 계산합니다. 각 축에 대한 회전은 서로 다른 좌표에 영향을 줍니다.
- **수학식 (예: Z축 기준 $\theta$ 만큼 회전):**
    
    $$
    p'_x = p_x \cos(\theta) - p_y \sin(\theta) \\
    p'_y = p_x \sin(\theta) + p_y \cos(\theta) \\
    p'_z = p_z
    $$
    
    - 3D 회전은 X, Y, Z축에 대한 회전을 순차적으로 조합하여(오일러 각) 구현할 수 있습니다.

---

## 2. 변환 행렬 (Transformation Matrices)

### 2.1. 행렬의 필요성

위치, 회전, 크기 변환을 각각 순차적으로 적용하는 것은 비효율적입니다.

- **위치(덧셈)**, **크기/회전(곱셈)**은 서로 다른 연산입니다.
- 이 모든 연산을 **단 하나의 '행렬 곱셈'**으로 통합하면, 여러 변환을 미리 계산하여 하나의 행렬(TRS Matrix)로 만들어두고 모든 정점에 한 번만 적용할 수 있어 매우 효율적입니다.

### 2.2. 동차 좌표계 (Homogeneous Coordinates)

- **문제:** 3x3 행렬로는 회전(Rotation)과 크기(Scale)는 표현할 수 있지만, 이동(Position/Translation)은 행렬 곱셈으로 표현할 수 없습니다. (이동은 덧셈이기 때문)
- **해결:** **동차 좌표계**를 도입하여 3D 좌표( $[x, y, z]$ )를 4D 좌표( $[x, y, z, 1]$ )로 확장합니다.
- 이를 통해 덧셈이었던 '이동' 연산도 4x4 행렬의 '곱셈'으로 표현할 수 있게 됩니다.

### 2.3. 4x4 변환 행렬

`Matrix4x4`를 사용하여 위치, 회전, 크기 변환을 모두 포함하는 단일 행렬을 만듭니다.

- **이동 행렬 (Translation Matrix):**
    
    $$
    \begin{bmatrix}
    1 & 0 & 0 & t_x \\
    0 & 1 & 0 & t_y \\
    0 & 0 & 1 & t_z \\
    0 & 0 & 0 & 1
    \end{bmatrix}
    $$
    
- **회전 행렬 (Rotation Matrix):**

$$
\begin{bmatrix}
\cos Y \cos Z & -\cos Y \sin Z & \sin Y &0\\
\cos X \sin Z + \sin X \sin Y \cos Z & \cos X \cos Z - \sin X \sin Y \sin Z & -\sin X \cos Y &0\\
\sin X \sin Z - \cos X \sin Y \cos Z & \sin X \cos Z + \cos X \sin Y \sin Z & \cos X \cos Y &0 \\0&0&0&1
\end{bmatrix}
$$

- **크기 행렬 (Scale Matrix):**
    
    $$
    \begin{bmatrix}
    s_x & 0 & 0 & 0 \\
    0 & s_y & 0 & 0 \\
    0 & 0 & s_z & 0 \\
    0 & 0 & 0 & 1
    \end{bmatrix}
    $$
    
- **TRS 통합 행렬:** 이 행렬들을 모두 곱하여 최종 변환 행렬 $M$을 만듭니다.
    
    $$
    M = \text{Translation} \times \text{Rotation} \times \text{Scale} 
    $$
    
    (Unity에서는 이 순서(TRS)를 따릅니다.)
    
- **최종 계산:** $p' = M \cdot p$ (여기서 $p$는 $[x, y, z, 1]$ 벡터)

---

## 3. 투영 행렬 (Projection Matrices)

모든 물체가 3D 월드 공간에서 변환(Model → World)되고 카메라 시점(View)으로 변환된 후, 마지막으로 3D 씬을 2D 화면에 '투영'해야 합니다.

- **MVP 변환:** 렌더링 파이프라인의 핵심 변환입니다.
`최종 화면 좌표 = Projection × View × Model × 로컬 좌표`
- **투영의 역할:** 3D 카메라 공간의 점들을 2D 클립 공간(Clip Space)으로 변환합니다.

### 3.1. 직교 투영 (Orthographic Projection)

- **개념:** 원근감을 무시하는 투영 방식입니다. (예: 2D 게임, CAD 툴)
- **작동:** 3D 공간의 직육면체(View Frustum)를 2D 사각형으로 그대로 '압축'시킵니다.
- **특징:** 물체의 깊이(z값)가 화면상 크기에 영향을 주지 않습니다. 단순히 x, y 좌표를 사용하고 z(깊이) 정보는 앞뒤를 가리는 데만 사용합니다.

### 3.2. 원근 투영 (Perspective Projection)

- **개념:** 사람의 눈이나 카메라처럼 '원근감'을 적용하는 투영 방식입니다.
- **핵심 원리:** **"멀리 있는 물체는 작게 보인다."**
- **계산:** 이 효과를 구현하기 위해, 정점의 $x, y$ **좌표를 $z$ 좌표(깊이)로 나눕니다.**
    
    $$
    p'_x = \frac{p_x}{p_z} \quad , \quad p'_y = \frac{p_y}{p_z}
    $$
    
- **행렬과 나눗셈:**
    - **문제:** 기본 행렬 곱셈은 '나눗셈'을 직접 수행할 수 없습니다.
    - **해결 (동차 좌표계 활용):** 투영 행렬은 4D 벡터 $[x, y, z, 1]$을 곱한 결과로 $[x', y', z', w]$ 형태의 벡터를 만듭니다.
    - 이때 행렬은 $w$ 값이 원래의 $z$ 값이 되도록 설계됩니다.
    - **원근 나눗셈 (Perspective Divide):** 렌더링 파이프라인의 마지막 단계에서 GPU가 $w$ 값으로 모든 성분을 자동으로 나눕니다.
    
    $$
    [\frac{x'}{w}, \frac{y'}{w}, \frac{z'}{w}] \Rightarrow [\frac{x \cdot f}{z}, \frac{y \cdot f}{z}, \dots]
    $$
    
- **초점 거리 (Focal Length) / FOV:**
    - $z$ 로 나누는 정도(원근감의 강도)를 조절하기 위해 초점 거리( $f$ ) 또는 시야각(Field of View, FOV) 개념이 투영 행렬에 포함됩니다.
    - 이는 줌(Zoom) 기능이나 카메라 렌즈 효과를 구현하는 데 사용됩니다.