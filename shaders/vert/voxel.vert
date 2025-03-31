#version 330

layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec3 aNormal;
layout (location = 2) in vec2 aBary;
layout (location = 3) in int aTextureID;

out vec2 vUV;
out vec2 vBary;
flat out float vBrightness;

uniform mat4 mvp;
uniform vec3 worldPosition;

void main()
{
    vec3 vPosition = aPosition + worldPosition;
    vBary = aBary;

    int face;

    if (aNormal.y >= 0.5)
    {
        // FACE_YP
        face = 0;
        vBrightness = 1.0;
        vUV = vPosition.xz;
    }
    else if (aNormal.y <= -0.5)
    {
        // FACE_YN
        face = 1;
        vBrightness = 0.25;
        vUV = vPosition.xz;
    }
    else if (aNormal.x >= 0.5)
    {
        // FACE_XP
        face = 2;
        vBrightness = 0.75;
        vUV = vPosition.zy;
    }
    else if (aNormal.x <= -0.5)
    {
        // FACE_XN
        face = 3;
        vBrightness = 0.75;
        vUV = vPosition.zy;
    }
    else if (aNormal.z >= 0.5)
    {
        // FACE_ZP
        face = 4;
        vBrightness = 0.5;
        vUV = vPosition.xy;
    }
    else
    {
        // FACE_ZN
        face = 5;
        vBrightness = 0.5;
        vUV = vPosition.xy;
    }

    vUV *= 2.0;


    // World to screen pos
    gl_Position = mvp * vec4(vPosition, 1.0);
}