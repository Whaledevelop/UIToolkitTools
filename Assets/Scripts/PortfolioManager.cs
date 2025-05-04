using UnityEngine;
using UnityEngine.UIElements;

public class PortfolioManager : MonoBehaviour
{
    [SerializeField] private UIDocument uiDocument;
    [SerializeField] private Texture2D avatarTexture;

    private void OnEnable()
    {
        var root = uiDocument.rootVisualElement;
        
        // You might need to load and assign the avatar texture if not using a resource
        if (avatarTexture != null)
        {
            var photo = root.Q<VisualElement>("photo");
            photo.style.backgroundImage = new StyleBackground(avatarTexture);
        }

        // If you need to load any dynamic content, you can do it here
        // For example, loading skills or experience from a data file
        
        // Example of how you might populate content dynamically:
        // PopulateSkills(root.Q("leftColumn"));
        // PopulateExperience(root.Q("rightColumn"));
    }

    // Example method to populate skills dynamically if needed
    private void PopulateSkills(VisualElement container)
    {
        // Example skills - replace with actual data source
        string[] skills = new string[] 
        { 
            "C#", 
            "Unity", 
            "UI Toolkit", 
            "DOTween", 
            "Shader Programming" 
        };

        foreach (var skill in skills)
        {
            var skillLabel = new Label($"• {skill}");
            skillLabel.AddToClassList("skill-item");
            container.Add(skillLabel);
        }
    }

    // Example method to populate experience dynamically if needed
    private void PopulateExperience(VisualElement container)
    {
        // Example experience - replace with actual data source
        var experiences = new[]
        {
            new { Company = "Game Studio", Position = "Senior Unity Developer", Period = "2020-Present", Description = "Leading development of AAA mobile games." },
            new { Company = "Tech Startup", Position = "Unity Developer", Period = "2018-2020", Description = "Built AR/VR applications for education." }
        };

        foreach (var exp in experiences)
        {
            var companyLabel = new Label(exp.Company);
            companyLabel.AddToClassList("company-name");
            
            var positionLabel = new Label($"{exp.Position} ({exp.Period})");
            positionLabel.AddToClassList("position-title");
            
            var descriptionLabel = new Label(exp.Description);
            descriptionLabel.AddToClassList("description-text");
            
            container.Add(companyLabel);
            container.Add(positionLabel);
            container.Add(descriptionLabel);
        }
    }
}