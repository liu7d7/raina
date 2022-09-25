#version 330 core

in vec2 v_TexCoords;
in vec2 v_Pos;

uniform sampler2D _tex0;
uniform float _width;
uniform float _threshold;
uniform vec4 _outlineColor;
uniform int _blackAndWhite;
uniform int _abs;
uniform int _glow;
uniform vec4 _otherColor;
uniform vec2 _screenSize;

out vec4 fragColor;

bool shouldOutline(vec2 pos, vec4 center) {
    const float sqrt2 = sqrt(2.0);
    float diag = _width / sqrt2;
    vec2 corners[8] = { (pos.xy - diag) / _screenSize, 
                        (vec2(pos.x + diag, pos.y - diag)) / _screenSize,
                        (vec2(pos.x - diag, pos.y + diag)) / _screenSize,
                        (pos.xy + diag) / _screenSize,
                        (vec2(pos.x - _width, pos.y)) / _screenSize,
                        (vec2(pos.x + _width, pos.y)) / _screenSize,
                        (vec2(pos.x, pos.y - _width)) / _screenSize,
                        (vec2(pos.x, pos.y + _width)) / _screenSize };
    float diff;
    float maxDiff = 0;
    for (int i = 0; i < 8; i++) {
        if (corners[i].x < 0 || corners[i].x > 1 || corners[i].y < 0 || corners[i].y > 1) {
            continue;
        }
        if (_abs == 0) {
            diff = -(center.r - texture(_tex0, corners[i]).r)
            -(center.g - texture(_tex0, corners[i]).g)
            -(center.b - texture(_tex0, corners[i]).b);
        } else {
            diff = abs(center.r - texture(_tex0, corners[i]).r)
            + abs(center.g - texture(_tex0, corners[i]).g)
            + abs(center.b - texture(_tex0, corners[i]).b);
        }
        if (diff > maxDiff) {
            maxDiff = diff;
        }
    }
    return maxDiff > _threshold / 2.0;
}

void main() {
    vec4 center = texture(_tex0, v_TexCoords);
    bool o = shouldOutline(v_Pos, center);
    bool o2[4] = { shouldOutline(v_Pos + vec2(-1, -1), center), 
                   shouldOutline(v_Pos + vec2(1, -1), center), 
                   shouldOutline(v_Pos + vec2(-1, 1), center), 
                   shouldOutline(v_Pos + vec2(1, 1), center) };
    float o3 = 0;
    if (_glow == 1) { 
        for (int i = 0; i < 4; i++) {
            if (o2[i]) {
                o3 += 0.25;
            }
        } 
    }
    if (_blackAndWhite == 1) {
        if (o) {
            fragColor = _outlineColor;
        } else {
            fragColor = mix(_otherColor, _outlineColor, vec4(o3));
        }
    } else {
        if (o) {
            fragColor = _outlineColor;
        } else {
            fragColor = mix(center, _outlineColor, vec4(o3));
        }
    }
}