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
    public List<Subject>[][] spots;
    private List<(SubjectType, SubjectType)> _prevRules;

    private ObjectFitContain _contain; 
    
    void Start()
    {
        string levelFile = $"Levels/{levelId}";
        TextAsset text = Resources.Load<TextAsset>(levelFile);
        string[] lines = text.text.Split('\n');

        width = int.Parse(lines[0]);
        height = int.Parse(lines[1]);

        spots = new List<Subject>[height][];

        for (int y = 0; y < height; y++)
        {
            spots[y] = new List<Subject>[width];
            for (int x = 0; x < width; x++)
            {
                spots[y][x] = new List<Subject>(2); // entities rarely stack in more than 2
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
                    
                var obj = new GameObject();
                obj.transform.parent = transform;
                var subject = obj.AddComponent<Subject>();
                subject.SetEntityType(type);
                subject.x = x;
                subject.y = y;
                subject.z = 0;
                spots[y][x].Add(subject);
            }
        }
        
        _prevRules = new List<(SubjectType, SubjectType)>();
        UpdateRules();
        ApplyInitialRules();

        _contain = gameObject.AddComponent<ObjectFitContain>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_contain.Contain(width, height))
        {
            // invert Y transform
            var localPos = gameObject.transform.localPosition;
            gameObject.transform.localPosition = new Vector3(localPos.x, -localPos.y, localPos.z);
        }
    }

    public bool IsValidSpot(int x, int y)
    {
        return x >= 0 && x < width && y >= 0 && y < height;
    }
    
    List<(SubjectType, SubjectType)> ExtractRules()
    {
        var output = new List<(SubjectType, SubjectType)>();
        
        foreach (var row in spots)
        {
            foreach (var column in row)
            {
                foreach (Subject entity in column)
                {
                    if (entity.GetEntityType() == SubjectType.ConnectorIs)
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
            ExtractRulesFromSpots(spots[subject.y - 1][subject.x], spots[subject.y + 1][subject.x], list);
        }
        
        // is connector can be a part of a horizontal rule
        if (subject.x != 0 && subject.x != width - 1)
        {
            ExtractRulesFromSpots(spots[subject.y][subject.x - 1], spots[subject.y][subject.x + 1], list);
        }
    }

    private void ExtractRulesFromSpots(List<Subject> primarySpot, List<Subject> secondarySpot, List<(SubjectType, SubjectType)> list)
    {
        foreach (var primary in primarySpot)
        {
            var primaryType = primary.GetEntityType();
            var isPrimarySubject = Subject.IsSubject(primaryType);
            var isPrimaryTrait = Subject.IsTrait(primaryType);

            if (!isPrimarySubject && !isPrimaryTrait) continue;
                
            foreach (var secondary in secondarySpot)
            {
                var secondaryType = secondary.GetEntityType();
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
                if (entity.GetEntityType() != targetType) continue;
                
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
                if (entity.GetEntityType() != targetType) continue;

                entity.gameObject.AddComponent(Subject.GetTraitBehavior(trait));
            }
        }

        _prevRules = newRules;
    }

    private void ApplyInitialRules()
    {
        foreach (var row in spots)
        {
            foreach (var spot in row)
            {
                foreach (var entity in spot)
                {
                    var type = entity.GetEntityType();
                    if (Subject.IsSubject(type) || Subject.IsTrait(type) || type == SubjectType.ConnectorIs)
                    {
                        entity.gameObject.AddComponent<Traits.Push>();
                    }
                }
            }
        }
    }
}
