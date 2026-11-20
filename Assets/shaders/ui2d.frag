#version 330 core
out vec4 FragColor;
in vec2 TexCoord;

uniform sampler2D u_Texture;
uniform float u_Alpha;

void main() {
    vec4 color = texture(u_Texture, TexCoord);
    FragColor = vec4(color.rgb, color.a * u_Alpha);
}
