2번째 Light를 계산하기 위해선 LightMode가 ForwardAdd인 별도의 Pass를 작성해야한다.
단 Blend One One을 설정하지 않으면 새로운 라이트가 단순히 덮어씌어진다

Light는 Directional, Point, Spot과 같은 다양한 종류가 있음.

Light Cookie를 이용하여 디테일한 조명 생성 가능
-----------------------------------------------------------------------
[Vertex Light]
덜 중요하거나 멀리 떨어져있는 라이트는 픽셀 단위가 아닌 Vertex 단위로 조명을 계산함
-> Pixel Light보다 빠르지만 디테일은 부족함

[Spherical Harmonics]
가장 빠르지만 가장 부정확한(근사치) 계산법. 주변의 복잡한 조명 환경을 매우 단순한 Low Frequency 데이터로 압축하여 표현함 