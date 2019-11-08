using System;
using System.Collections.Generic;
using System.Linq;
using Subjects;
using UnityEngine;

/**
 * Loads map from level_.txt file, renders tiles, parses and applies rules.
 * IMPORTANT: tiles exist in a plane where Y axis grows downwards (like in images)
 */
public class Map : MonoBehaviour
{
    public int levelId;
    public int width;
    public int height;
    public List<Subject>[][] stacks;
    private List<(SubjectType, SubjectType)> _prevRules;

    private ObjectFitContain _contain;

    void Start()
    {
        string levelFile = $"Levels/{levelId}";
        TextAsset text = Resources.Load<TextAsset>(levelFile);
        string[] lines = text.text.Split('\n');

        width = int.Parse(lines[0]);
        height = int.Parse(lines[1]);

        var mapBackground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        mapBackground.name = "background";
        mapBackground.transform.parent = gameObject.transform;
        mapBackground.transform.localPosition = new Vector3(width / 2.0f, -height / 2.0f, 1);
        
        mapBackground.transform.localScale = new Vector3(width / 10.0f, 1, height / 10.0f);
        
        var rotation = mapBackground.transform.localRotation.eulerAngles;
        rotation.x = -90;
        mapBackground.transform.localRotation = Quaternion.Euler(rotation);
        
        mapBackground.GetComponent<MeshRenderer>().material.color = new Color(0.08f, 0.08f, 0.08f, 1f);
        mapBackground.GetComponent<MeshRenderer>().material.shader = Shader.Find("UI/Default");

        stacks = new List<Subject>[height][];
        
        for (int y = 0; y < height; y++)
        {
            stacks[y] = new List<Subject>[width];
            for (int x = 0; x < width; x++)
            {
                stacks[y][x] = new List<Subject>(2); // entities rarely stack in more than 2
                var id = int.Parse(lines[2].Substring((width * y + x) * 2, 2));
                
                // we skip id 0, which encodes an empty tile
                if (id == 0) continue;
                
                SubjectType type;
                    
                try
                {
                    type = (SubjectType) id;
                }
                catch (InvalidCastException)
                {
                    Debug.LogWarning($"Invalid entity type {id} in map file {levelFile}");
                    continue;
                }

                var tile = MakeTile(x, y, type);
                stacks[y][x].Add(tile.GetComponent<Subject>());
            }
        }
        
        _prevRules = new List<(SubjectType, SubjectType)>();
        UpdateRules();
        ApplyInitialRules();

        _contain = gameObject.AddComponent<ObjectFitContain>();
    }

    void Update()
    {
        if (_contain.Contain(width, height))
        {
            // invert Y transform
            var localPos = gameObject.transform.localPosition;
            gameObject.transform.localPosition = new Vector3(localPos.x, -localPos.y, localPos.z);
        }
    }
    
    private GameObject MakeTile(int x, int y, SubjectType type)
    {
        var obj = new GameObject("subject");
        obj.transform.parent = gameObject.transform;
        
        var subject = obj.AddComponent<Subject>();
        subject.SetEntityType(type);
        subject.x = x;
        subject.y = y;
        subject.z = 0;
        
        var ren = obj.AddComponent<SpriteRenderer>();
        ren.sprite = Sprite.Create(
            Subject.GetTexture(),
            new Rect(32 * (int) (type-1), 0, 32, 32),
            new Vector2(0, 0),
            32);

        obj.AddComponent<AnimatedSprite>();

        var mod = Subject.GetModTilemap(type);

        if (mod != null)
        {
            var modComp = obj.AddComponent<TileMod>();
            modComp.modTilemap = mod;
        }

        return obj;
    }


    public bool IsValidSpot(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < width && pos.y >= 0 && pos.y < height;
    }

    public byte CollectNeighborsByte(Subject s, bool bitForMissingNeighbors = false)
    {
        
        SubjectType type = s.GetSubjectType();

        bool CollectStack(int x, int y)
        {
            if (!IsValidSpot(new Vector2Int(x, y))) return bitForMissingNeighbors;
            return stacks[y][x].Find(subject => subject.GetSubjectType() == type);
        }
        
        byte output = 0;
        
        output |= (byte)(CollectStack(s.x - 1, s.y - 1) ?   1 : 0);
        output |= (byte)(CollectStack(s.x + 0, s.y - 1) ?   2 : 0);
        output |= (byte)(CollectStack(s.x + 1, s.y - 1) ?   4 : 0);
        
        output |= (byte)(CollectStack(s.x - 1, s.y + 0) ?   8 : 0);
        
        output |= (byte)(CollectStack(s.x + 1, s.y + 0) ?  16 : 0);
        
        output |= (byte)(CollectStack(s.x - 1, s.y + 1) ?  32 : 0);
        output |= (byte)(CollectStack(s.x + 0, s.y + 1) ?  64 : 0);
        output |= (byte)(CollectStack(s.x + 1, s.y + 1) ? 128 : 0);

        return output;
    }

    List<(SubjectType, SubjectType)> ExtractRules()
    {
        var output = new List<(SubjectType, SubjectType)>();
        
        foreach (var row in stacks)
        {
            foreach (var column in row)
            {
                foreach (Subject entity in column)
                {
                    if (entity.GetSubjectType() == SubjectType.ConnectorIs)
                    {
                        ExtractRulesFromConnectorIs(entity, output);
                    }
                }
            }
        }

        return output;
    }

    private void ExtractRulesFromConnectorIs(Subject subject, List<(SubjectType, SubjectType)> list)
    {
        // is connector can be a part of a vertical rule
        if (subject.y != 0 && subject.y != height - 1)
        {
            ExtractRulesFromSpots(stacks[subject.y - 1][subject.x], stacks[subject.y + 1][subject.x], list);
        }
        
        // is connector can be a part of a horizontal rule
        if (subject.x != 0 && subject.x != width - 1)
        {
            ExtractRulesFromSpots(stacks[subject.y][subject.x - 1], stacks[subject.y][subject.x + 1], list);
        }
    }

    private void ExtractRulesFromSpots(List<Subject> primarySpot, List<Subject> secondarySpot, List<(SubjectType, SubjectType)> list)
    {
        foreach (var primary in primarySpot)
        {
            var primaryType = primary.GetSubjectType();
            var isPrimarySubject = Subject.IsSubject(primaryType);
            var isPrimaryTrait = Subject.IsTrait(primaryType);

            if (!isPrimarySubject && !isPrimaryTrait) continue;
                
            foreach (var secondary in secondarySpot)
            {
                var secondaryType = secondary.GetSubjectType();
                var isSecondarySubject = Subject.IsSubject(secondaryType);
                var isSecondaryTrait = Subject.IsTrait(secondaryType);

                if (isPrimarySubject && isSecondaryTrait)
                {
                    list.Add((primaryType, secondaryType));
                }

                if (isSecondarySubject && isPrimaryTrait)
                {
                    list.Add((secondaryType, primaryType));
                }
            }
        }
    }

    public void UpdateRules()
    {
        var newRules = ExtractRules();
        var addedRules = newRules.Except(_prevRules);
        var deletedRules = _prevRules.Except(newRules);

        foreach (var (subject, trait) in deletedRules)
        {
            var targetType = Subject.GetSubjectType(subject);
            var entities = gameObject.GetComponentsInChildren<Subject>();

            foreach (var entity in entities)
            {
                if (entity.GetSubjectType() != targetType) continue;
                
                var component = entity.gameObject.GetComponent(Subject.GetTraitBehavior(trait));
                if (component) Destroy(component);
            }
        }
        
        foreach (var (subject, trait) in addedRules)
        {
            var targetType = Subject.GetSubjectType(subject);
            var entities = gameObject.GetComponentsInChildren<Subject>();

            foreach (var entity in entities)
            {
                if (entity.GetSubjectType() != targetType) continue;

                entity.gameObject.AddComponent(Subject.GetTraitBehavior(trait));
            }
        }

        _prevRules = newRules;
    }

    private void ApplyInitialRules()
    {
        foreach (var row in stacks)
        {
            foreach (var spot in row)
            {
                foreach (var entity in spot)
                {
                    var type = entity.GetSubjectType();
                    if (Subject.IsSubject(type) || Subject.IsTrait(type) || type == SubjectType.ConnectorIs)
                    {
                        entity.gameObject.AddComponent<Traits.Push>();
                    }
                }
            }
        }
    }

    public void RefreshPositions(List<Vector2Int> positions)
    {
        var affectedTiles = new HashSet<(int, int)>();

        void TryAddPosition(Vector2Int pos)
        {
            if (IsValidSpot(pos)) affectedTiles.Add((pos.x, pos.y));
        }

        foreach (var position in positions)
        {
            TryAddPosition(position + Vector2Int.left   + Vector2Int.down);
            TryAddPosition(position                     + Vector2Int.down);
            TryAddPosition(position + Vector2Int.right  + Vector2Int.down);
            
            TryAddPosition(position + Vector2Int.left);
            TryAddPosition(position);
            TryAddPosition(position + Vector2Int.right);
            
            TryAddPosition(position + Vector2Int.left   + Vector2Int.up);
            TryAddPosition(position                     + Vector2Int.up);
            TryAddPosition(position + Vector2Int.right  + Vector2Int.up);
        }

        foreach (var (x, y) in affectedTiles)
        {
            foreach (var subject in stacks[y][x])
            {
                subject.Refresh();
            }
        }
    }
}
