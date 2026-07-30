#version 330 core
in vec2 TextCoord;
in vec4 VertexColor;

out vec4 FragColor;

uniform sampler2D uTexture;

void main() {
    vec4 sampled = vec4(1.0, 1.0, 1.0, texture(uTexture, TextCoord).a);
    FragColor = sampled * VertexColor;
}