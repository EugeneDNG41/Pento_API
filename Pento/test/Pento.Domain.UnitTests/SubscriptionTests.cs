using Pento.Domain.Abstractions;
using Pento.Domain.Subscriptions;

namespace Pento.Domain.UnitTests;


internal sealed class SubscriptionTests
{
    /// <summary>
    /// Verifies that UpdateDetails updates Name, Description and IsActive when non-whitespace name and description
    /// and a different isActive value are provided.
    /// Input: subscription with initial values; name = "newName", description = "newDesc", isActive = different value.
    /// Expected: Name and Description updated to provided values; IsActive updated to new value.
    /// </summary>
    [Test]
    public void UpdateDetails_AllFieldsUpdated_WhenValidInputsProvided()
    {
        // Arrange
        var initialId = Guid.NewGuid();
        var sut = new Subscription(initialId, "oldName", "oldDesc", true);

        // Act
        sut.UpdateDetails("newName", "newDesc", false);

        // Assert
        Assert.That(sut.Name, Is.EqualTo("newName"));
        Assert.That(sut.Description, Is.EqualTo("newDesc"));
        Assert.That(sut.IsActive, Is.False);
    }

    /// <summary>
    /// Verifies that providing null or whitespace for name does not change the existing Name.
    /// Input: name = null/empty/whitespace; description and isActive left null.
    /// Expected: Name remains unchanged.
    /// </summary>
    [TestCase(null)]
    [TestCase("")]
    [TestCase("   ")]
    public void UpdateDetails_NameNullOrWhiteSpace_NameUnchanged(string? inputName)
    {
        // Arrange
        var sut = new Subscription(Guid.NewGuid(), "initialName", "initialDesc", true);

        // Act
        sut.UpdateDetails(inputName, null, null);

        // Assert
        Assert.That(sut.Name, Is.EqualTo("initialName"));
    }

    /// <summary>
    /// Verifies that providing null or whitespace for description does not change the existing Description.
    /// Input: description = null/empty/whitespace; name and isActive left null.
    /// Expected: Description remains unchanged.
    /// </summary>
    [TestCase(null)]
    [TestCase("")]
    [TestCase("   ")]
    public void UpdateDetails_DescriptionNullOrWhiteSpace_DescriptionUnchanged(string? inputDescription)
    {
        // Arrange
        var sut = new Subscription(Guid.NewGuid(), "initialName", "initialDescription", true);

        // Act
        sut.UpdateDetails(null, inputDescription, null);

        // Assert
        Assert.That(sut.Description, Is.EqualTo("initialDescription"));
    }

    /// <summary>
    /// Verifies that providing a null isActive does not change the existing IsActive.
    /// Input: isActive = null.
    /// Expected: IsActive remains unchanged.
    /// </summary>
    [Test]
    public void UpdateDetails_NullIsActive_IsActiveUnchanged()
    {
        // Arrange
        var sut = new Subscription(Guid.NewGuid(), "n", "d", false);

        // Act
        sut.UpdateDetails(null, null, null);

        // Assert
        Assert.That(sut.IsActive, Is.False);
    }

    /// <summary>
    /// Verifies that providing an isActive value equal to current value does not change IsActive.
    /// Input: isActive set to the current value.
    /// Expected: IsActive remains unchanged.
    /// </summary>
    [TestCase(true)]
    [TestCase(false)]
    public void UpdateDetails_IsActiveSameValue_NoChange(bool initial)
    {
        // Arrange
        var sut = new Subscription(Guid.NewGuid(), "n", "d", initial);

        // Act
        sut.UpdateDetails(null, null, initial);

        // Assert
        Assert.That(sut.IsActive, Is.EqualTo(initial));
    }

    /// <summary>
    /// Verifies that providing an isActive value different from current value updates IsActive.
    /// Input: isActive set to a different value than current.
    /// Expected: IsActive updated to provided value.
    /// </summary>
    [TestCase(true, false)]
    [TestCase(false, true)]
    public void UpdateDetails_IsActiveDifferent_IsUpdated(bool initial, bool newValue)
    {
        // Arrange
        var sut = new Subscription(Guid.NewGuid(), "n", "d", initial);

        // Act
        sut.UpdateDetails(null, null, newValue);

        // Assert
        Assert.That(sut.IsActive, Is.EqualTo(newValue));
    }

    /// <summary>
    /// Verifies that very long and special-character strings are accepted and update Name/Description.
    /// Input: name/description set to long and special-character strings.
    /// Expected: Name/Description updated to exact provided strings.
    /// </summary>
    [Test]
    public void UpdateDetails_LongAndSpecialCharacterStrings_UpdatesSuccessfully()
    {
        // Arrange
        string longString = new string('x', 10000);
        string specialString = "Line1\nLine2\r\n\u0001\u0002\u263A";
        var sut = new Subscription(Guid.NewGuid(), "old", "old", true);

        // Act
        sut.UpdateDetails(longString, specialString, null);

        // Assert
        Assert.That(sut.Name, Is.EqualTo(longString));
        Assert.That(sut.Description, Is.EqualTo(specialString));
    }

    /// <summary>
    /// Provides a set of valid constructor inputs exercising edge-case strings and GUIDs.
    /// Cases:
    /// - Guid.Empty with basic strings and true.
    /// - Random Guid with empty strings and false.
    /// - Random Guid with whitespace/control-character strings and true.
    /// - Random Guid with very long strings and false.
    /// - Random Guid with special/unicode characters and true.
    /// Expected: Constructor creates instance and properties exactly match the inputs.
    /// </summary>
    private static IEnumerable<TestCaseData> Constructor_ValidInputs_Cases()
    {
        yield return new TestCaseData(Guid.Empty, "BasicName", "BasicDescription", true)
            .SetName("EmptyGuid_BasicStrings_True");
        yield return new TestCaseData(Guid.NewGuid(), string.Empty, string.Empty, false)
            .SetName("RandomGuid_EmptyStrings_False");
        yield return new TestCaseData(Guid.NewGuid(), "   ", "\t\n", true)
            .SetName("RandomGuid_WhitespaceAndControlChars_True");
        yield return new TestCaseData(Guid.NewGuid(), new string('x', 1024), new string('y', 2048), false)
            .SetName("RandomGuid_VeryLongStrings_False");
        yield return new TestCaseData(Guid.NewGuid(), "Special!@#$%^&*()", "Unicode: ñ ö 漢字", true)
            .SetName("RandomGuid_SpecialAndUnicode_True");
    }

    /// <summary>
    /// Verifies that the Subscription constructor assigns Name, Description and IsActive exactly as provided.
    /// Input conditions: various GUIDs (including Guid.Empty), names and descriptions (empty, whitespace, long, special),
    /// and both boolean values for isActive are tested via TestCaseSource.
    /// Expected: The created Subscription instance is not null and its public properties equal the constructor inputs.
    /// </summary>
    [Test, TestCaseSource(nameof(Constructor_ValidInputs_Cases))]
    public void Ctor_ValidInputs_SetsProperties(Guid id, string name, string description, bool isActive)
    {
        // Arrange
        // (No additional setup required for this constructor-only test.)

        // Act
        var subscription = new Subscription(id, name, description, isActive);

        // Assert
        Assert.That(subscription, Is.Not.Null, "Constructor should return a non-null instance for valid inputs.");
        Assert.That(subscription.Name, Is.EqualTo(name), "Name property should match the constructor argument.");
        Assert.That(subscription.Description, Is.EqualTo(description), "Description property should match the constructor argument.");
        Assert.That(subscription.IsActive, Is.EqualTo(isActive), "IsActive property should match the constructor argument.");
    }

    /// <summary>
    /// Provides diverse valid input combinations for the Create method.
    /// Cases include empty strings, whitespace-only, long strings, and special characters,
    /// combined with both boolean values for isActive.
    /// </summary>
    private static IEnumerable<TestCaseData> CreateCases()
    {
        string longString = new string('a', 1000);
        yield return new TestCaseData("Standard", "Standard description", true).SetName("Standard_NonEmptyStrings_True");
        yield return new TestCaseData("", "", false).SetName("EmptyNameAndDescription_False");
        yield return new TestCaseData("   ", "\t\n", true).SetName("WhitespaceNameAndControlChars_True");
        yield return new TestCaseData(longString, longString, false).SetName("VeryLongStrings_False");
        yield return new TestCaseData("Special\u0001\u0002", "Emojis😀🚀", true).SetName("SpecialAndEmoji_True");
    }

    /// <summary>
    /// Verifies that Create returns a Subscription instance whose Name, Description and IsActive
    /// properties match the provided inputs and that an Id is generated (non-empty).
    /// Input: various non-null string values (including empty, whitespace, long and special strings) and boolean flags.
    /// Expected: Returned Subscription properties equal inputs and Id is not Guid.Empty.
    /// </summary>
    [Test, TestCaseSource(nameof(CreateCases))]
    public void Create_ValidInputs_ReturnsSubscriptionWithPropertiesCorrect(string name, string description, bool isActive)
    {
        // Arrange
        // (Inputs provided by TestCaseSource)

        // Act
        var subscription = Subscription.Create(name, description, isActive);

        // Assert
        Assert.That(subscription, Is.Not.Null, "Expected Create to return a non-null Subscription instance.");
        // Assert properties
        Assert.That(subscription.Name, Is.EqualTo(name), "Expected Name to match the provided input.");
        Assert.That(subscription.Description, Is.EqualTo(description), "Expected Description to match the provided input.");
        Assert.That(subscription.IsActive, Is.EqualTo(isActive), "Expected IsActive to match the provided input.");

        // Attempt to assert that an Id was generated and is not Guid.Empty.
        // The Id is provided by the base Entity; ensure it's present and non-empty.
        // If Entity.Id is not publicly accessible this assertion will fail to compile and should be updated accordingly.
        Assert.That(((Entity)subscription).Id, Is.Not.EqualTo(Guid.Empty), "Expected a non-empty Guid to be generated for Id.");
    }

    /// <summary>
    /// Verifies that multiple calls to Create produce distinct Subscription instances with different Ids.
    /// Input: same name/description/isActive called twice.
    /// Expected: Two different Subscription instances have non-empty and different Id values.
    /// </summary>
    [Test]
    public void Create_MultipleCalls_ReturnDistinctIds()
    {
        // Arrange
        string name = "Standard";
        string description = "Repeated creation test";
        bool isActive = true;

        // Act
        var first = Subscription.Create(name, description, isActive);
        var second = Subscription.Create(name, description, isActive);

        // Assert
        Assert.That(first, Is.Not.Null, "First instance should not be null.");
        Assert.That(second, Is.Not.Null, "Second instance should not be null.");

        // Validate Ids are generated and distinct.
        Assert.That(((Entity)first).Id, Is.Not.EqualTo(Guid.Empty), "First Id should be non-empty.");
        Assert.That(((Entity)second).Id, Is.Not.EqualTo(Guid.Empty), "Second Id should be non-empty.");
        Assert.That(((Entity)first).Id, Is.Not.EqualTo(((Entity)second).Id), "Each call to Create should produce a unique Id.");
    }
}
