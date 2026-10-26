#version 330 core
in vec3 FragPos;
in vec3 Normal;

uniform vec3 objectColor;
uniform vec3 viewPos;

out vec4 FragColor;

struct Light {
    vec3 position;
    float constant;
    vec3 color;
    float linear;
    vec3 ambient;
    float quadratic;
};

layout(std140) uniform LightBlock {
    Light lights[64];
    int lightCount;
    int _pad1;
    int _pad2;
    int _pad3;
};

void main() {
    vec3 norm = normalize(Normal);
    vec3 viewDir = normalize(viewPos - FragPos);
    vec3 finalColor = vec3(0.0);

    for(int i = 0; i < lightCount; i++) {
        Light light = lights[i];
        
        vec3 lightDir = normalize(light.position - FragPos);
        float distance = length(light.position - FragPos);
        float attenuation = 1.0 / (light.constant + light.linear * distance + light.quadratic * (distance * distance));
        
        vec3 ambient = light.ambient * attenuation;
        
        float diff = max(dot(norm, lightDir), 0.0);
        vec3 diffuse = diff * light.color * attenuation;
        
        vec3 halfwayDir = normalize(lightDir + viewDir);
        float spec = pow(max(dot(norm, halfwayDir), 0.0), 32.0);
        vec3 specular = spec * light.color * attenuation;
        
        finalColor += (ambient + diffuse + specular) * objectColor;
    }

    FragColor = vec4(clamp(finalColor, 0.0, 1.0), 1.0);
}