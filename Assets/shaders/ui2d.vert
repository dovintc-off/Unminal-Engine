#version 330 core

vec2 vertices[4] = vec2[](
    vec2(0.0, 0.0),
    vec2(1.0, 0.0),
    vec2(1.0, 1.0),
    vec2(0.0, 1.0)
);

out vec2 TexCoord;

uniform vec2 u_Position;
uniform vec2 u_TextureSize;
uniform vec2 u_ScreenSize;

void main() {
    vec2 baseVertex = vertices[gl_VertexID];
    TexCoord = vec2(baseVertex.x, 1.0 - baseVertex.y);

    vec2 pixelPos = u_Position + (baseVertex * u_TextureSize);
    vec2 zeroToOne = pixelPos / u_ScreenSize;
    vec2 ndcPos = (zeroToOne * 2.0) - vec2(1.0);
    ndcPos.y = -ndcPos.y; 

    gl_Position = vec4(ndcPos, 0.0, 1.0);
}