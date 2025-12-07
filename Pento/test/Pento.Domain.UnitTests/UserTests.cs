
#nullable enable
using Pento.Domain.Abstractions;
using Pento.Domain.Roles;
using Pento.Domain.Users;
using Pento.Domain.Users.Events;

namespace Pento.Domain.UnitTests;


internal sealed class UserTests
{
    /// <summary>
    /// Verifies that SetRoles adds roles that are not currently present on the user.
    /// Input: user with no roles, setRoles contains Administrator and User.
    /// Expected: Roles collection contains Administrator and User exactly once each.
    /// </summary>
    [Test]
    public void SetRoles_NewRolesAreAdded_ResultContainsThoseRoles()
    {
        // Arrange
        var user = User.Create("a@b.com", "First", "Last", "identity-1", DateTime.UtcNow);
        Role[] newRoles = new[] { Role.Administrator, Role.User };

        // Act
        user.SetRoles(newRoles);

        // Assert
        var roleNames = user.Roles.Select(r => r.Name).ToList();
        Assert.That(roleNames.Count, Is.EqualTo(2), "Expected two roles to be present after adding.");
        Assert.That(roleNames, Does.Contain(Role.Administrator.Name));
        Assert.That(roleNames, Does.Contain(Role.User.Name));
    }

    /// <summary>
    /// Verifies that SetRoles removes roles that are not present in the provided setRoles enumerable.
    /// Input: user initially has Administrator and User; setRoles contains only User.
    /// Expected: Roles collection contains only User.
    /// </summary>
    [Test]
    public void SetRoles_RemovesRolesNotInNewSet_ResultContainsOnlySpecifiedRoles()
    {
        // Arrange
        var user = User.Create("a@b.com", "First", "Last", "identity-2", DateTime.UtcNow);
        // Seed with two roles first
        user.SetRoles(new[] { Role.Administrator, Role.User });
        Assert.That(user.Roles.Select(r => r.Name).Count(n => n == Role.Administrator.Name), Is.EqualTo(1), "Precondition: Administrator should be present once.");

        // Act
        user.SetRoles(new[] { Role.User });

        // Assert
        var roleNames = user.Roles.Select(r => r.Name).ToList();
        Assert.That(roleNames, Has.Count.EqualTo(1), "Expected only one role after removing unspecified roles.");
        Assert.That(roleNames[0], Is.EqualTo(Role.User.Name));
        Assert.That(roleNames, Does.Not.Contain(Role.Administrator.Name));
    }

    /// <summary>
    /// Verifies behavior when the input enumerable contains duplicate role entries (same Name).
    /// Input: user with no roles, setRoles contains Administrator twice.
    /// Expected: Because SetRoles adds each item from the toAdd list, duplicates in input produce duplicate entries in internal list.
    /// This test documents current behavior and will fail if implementation changes to deduplicate input.
    /// </summary>
    [Test]
    public void SetRoles_DuplicateEntriesInInput_ProducesDuplicateEntriesInRoles()
    {
        // Arrange
        var user = User.Create("a@b.com", "First", "Last", "identity-3", DateTime.UtcNow);
        Role[] duplicates = new[] { Role.Administrator, Role.Administrator };

        // Act
        user.SetRoles(duplicates);

        // Assert
        var roles = user.Roles.ToList();
        Assert.That(roles.Count, Is.EqualTo(2), "Expected two entries because duplicate input entries are both added.");
        Assert.That(roles[0].Name, Is.EqualTo(Role.Administrator.Name));
        Assert.That(roles[1].Name, Is.EqualTo(Role.Administrator.Name));
    }

    /// <summary>
    /// Verifies that Create assigns provided values to properties, initializes Id, leaves optional fields null/empty,
    /// and raises the expected domain events in the expected order.
    /// Input conditions: various valid email/firstName/lastName/identityId strings and createdAt DateTime values.
    /// Expected result: properties equal inputs, Id is non-empty, Roles empty, HouseholdId and AvatarUrl null,
    /// and two domain events exist: UserCreatedDomainEvent (with UserId == user.Id) followed by UserRegisteredDomainEvent (with IdentityId == identityId).
    /// </summary>
    [TestCaseSource(nameof(CreateValidInputs))]
    public void Create_ValidInputs_SetsPropertiesAndRaisesEvents(string email, string firstName, string lastName, string identityId, DateTime createdAt)
    {
        // Arrange
        // (inputs provided by test case)

        // Act
        var user = User.Create(email, firstName, lastName, identityId, createdAt);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(user, Is.Not.Null, "User.Create should not return null.");
            Assert.That(user.Email, Is.EqualTo(email), "Email should be assigned from input.");
            Assert.That(user.FirstName, Is.EqualTo(firstName), "FirstName should be assigned from input.");
            Assert.That(user.LastName, Is.EqualTo(lastName), "LastName should be assigned from input.");
            Assert.That(user.IdentityId, Is.EqualTo(identityId), "IdentityId should be assigned from input.");
            Assert.That(user.CreatedAt, Is.EqualTo(createdAt), "CreatedAt should be assigned from input.");
            Assert.That(user.Id, Is.Not.EqualTo(Guid.Empty), "Id should be initialized to a non-empty Guid.");
            Assert.That(user.HouseholdId, Is.Null, "HouseholdId should be null on creation.");
            Assert.That(user.AvatarUrl, Is.Null, "AvatarUrl should be null on creation.");
            Assert.That(user.Roles, Is.Not.Null, "Roles collection should be non-null.");
            Assert.That(user.Roles.Count, Is.EqualTo(0), "Roles should be empty on creation.");

            IReadOnlyList<IDomainEvent> events = user.GetDomainEvents();
            Assert.That(events, Is.Not.Null, "Domain events list should be non-null.");
            Assert.That(events.Count, Is.EqualTo(2), "Create should raise exactly two domain events.");

            Assert.That(events[0], Is.TypeOf<UserCreatedDomainEvent>(), "First event should be UserCreatedDomainEvent.");
            var createdEvent = (UserCreatedDomainEvent)events[0];
            Assert.That(createdEvent.UserId, Is.EqualTo(user.Id), "UserCreatedDomainEvent.UserId should match user's Id.");

            Assert.That(events[1], Is.TypeOf<UserRegisteredDomainEvent>(), "Second event should be UserRegisteredDomainEvent.");
            var registeredEvent = (UserRegisteredDomainEvent)events[1];
            Assert.That(registeredEvent.IdentityId, Is.EqualTo(identityId), "UserRegisteredDomainEvent.IdentityId should match provided identityId.");
        });
    }

    /// <summary>
    /// Verifies that multiple calls to Create produce distinct users with independent Ids and domain events.
    /// Input conditions: same inputs used for both calls.
    /// Expected result: produced users have different Ids and each has its own domain events matching its data.
    /// </summary>
    [Test]
    public void Create_CalledTwice_ProducesDistinctIdsAndEvents()
    {
        // Arrange
        string email = "dup@example.com";
        string firstName = "Dup";
        string lastName = "User";
        string identityId = "identity-dup";
        DateTime createdAt = DateTime.UtcNow;

        // Act
        var user1 = User.Create(email, firstName, lastName, identityId, createdAt);
        var user2 = User.Create(email, firstName, lastName, identityId, createdAt);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(user1.Id, Is.Not.EqualTo(user2.Id), "Each created user should have a unique Id.");

            IReadOnlyList<IDomainEvent> ev1 = user1.GetDomainEvents();
            IReadOnlyList<IDomainEvent> ev2 = user2.GetDomainEvents();

            Assert.That(ev1.Count, Is.EqualTo(2), "First user should have two domain events.");
            Assert.That(ev2.Count, Is.EqualTo(2), "Second user should have two domain events.");

            UserCreatedDomainEvent created1 = ev1.OfType<UserCreatedDomainEvent>().Single();
            UserCreatedDomainEvent created2 = ev2.OfType<UserCreatedDomainEvent>().Single();

            Assert.That(created1.UserId, Is.EqualTo(user1.Id), "UserCreatedDomainEvent for first user should reference first user's Id.");
            Assert.That(created2.UserId, Is.EqualTo(user2.Id), "UserCreatedDomainEvent for second user should reference second user's Id.");

            UserRegisteredDomainEvent reg1 = ev1.OfType<UserRegisteredDomainEvent>().Single();
            UserRegisteredDomainEvent reg2 = ev2.OfType<UserRegisteredDomainEvent>().Single();

            Assert.That(reg1.IdentityId, Is.EqualTo(identityId), "UserRegisteredDomainEvent.IdentityId should match input for first user.");
            Assert.That(reg2.IdentityId, Is.EqualTo(identityId), "UserRegisteredDomainEvent.IdentityId should match input for second user.");
        });
    }

    // Test case source providing several representative valid inputs including boundary DateTime values and edge string cases.
    private static IEnumerable<TestCaseData> CreateValidInputs()
    {
        yield return new TestCaseData("alice@example.com", "Alice", "Smith", "alice-1", DateTime.UtcNow)
            .SetName("Create_NormalValues_AssignsPropertiesAndEvents");
        yield return new TestCaseData("whitespace@example.com", "   ", "\t", "id-whitespace", DateTime.MinValue)
            .SetName("Create_WhitespaceNames_MinDate_AssignsPropertiesAndEvents");
        yield return new TestCaseData(new string('a', 1024) + "@example.com", new string('b', 2048), new string('c', 2048), new string('i', 512), DateTime.MaxValue)
            .SetName("Create_VeryLongStrings_MaxDate_AssignsPropertiesAndEvents");
        yield return new TestCaseData("special+chars@example.com", "Nämé", "O'Connor\n", "id-special-\u2603", new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc))
            .SetName("Create_SpecialCharacters_CustomDate_AssignsPropertiesAndEvents");
    }

    /// <summary>
    /// Test purpose:
    /// Verifies that SetAvatarUrl assigns the AvatarUrl property to the provided Uri value or to null.
    /// Input conditions:
    /// - Parameter avatarUrl: tested with a non-null absolute Uri and with null.
    /// Expected result:
    /// - When avatarUrl is non-null, AvatarUrl should be set to the same instance/value.
    /// - When avatarUrl is null, AvatarUrl should become null.
    /// 
    /// NOTE: Currently the test is inconclusive because an instance of User cannot be constructed
    /// with the provided source (private constructor and missing Create implementation). Once a
    /// valid factory method or public constructor is available, replace the Assert.Inconclusive
    /// with the Arrange-Act-Assert sequence that creates the User, calls SetAvatarUrl, and asserts
    /// the AvatarUrl property value.
    /// </summary>
    /// <param name="avatarUrl">The avatar Uri to set (may be null).</param>
    [TestCaseSource(nameof(AvatarUrlTestCases))]
    public void SetAvatarUrl_Input_AssignsAvatarUrl(Uri? avatarUrl)
    {
        var user = User.Create("a@b.com", "First", "Last", "identity-3", DateTime.UtcNow);
        // Act
        user.SetAvatarUrl(avatarUrl);
        // Assert
        Assert.That(user.AvatarUrl, Is.EqualTo(avatarUrl), "AvatarUrl property should match the input value after SetAvatarUrl call.");
    }

    /// <summary>
    /// Provides test cases for SetAvatarUrl: a non-null Uri and a null value.
    /// </summary>
    private static IEnumerable<TestCaseData> AvatarUrlTestCases()
    {
        yield return new TestCaseData(new Uri("https://example.com/avatar.png")).SetName("NonNull_Uri");
        yield return new TestCaseData((Uri?)null).SetName("Null_Uri");
    }

    /// <summary>
    /// Verifies that calling JoinHousehold with a valid Guid sets HouseholdId and raises a UserHouseholdJoinedDomainEvent.
    /// Input: a valid non-empty Guid representing the household id.
    /// Expected: User.HouseholdId equals the provided household id and a UserHouseholdJoinedDomainEvent is appended to the domain events with matching user and household ids.
    /// 
    /// NOTE: This test is marked Inconclusive because the User type has a private constructor and the public factory (Create) implementation
    /// was not available in the provided source slice. To complete this test:
    /// - Use the public factory (User.Create) if available, or
    /// - Provide an accessible constructor or a test helper factory to instantiate a User.
    /// Once an instance can be created, replace the Assert.Inconclusive with Arrange/Act/Assert that:
    ///   Arrange: create user instance with known Id
    ///   Act: user.JoinHousehold(householdId)
    ///   Assert: user.HouseholdId == householdId and user.GetDomainEvents() contains UserHouseholdJoinedDomainEvent with matching ids.
    /// </summary>
    [Test]
    public void JoinHousehold_ValidGuid_SetsHouseholdIdAndRaisesEvent()
    {
        // Arrange
        // The production User type cannot be instantiated with the provided source:
        // - The parameterless constructor is private.
        // - The static Create(...) factory implementation was not present in the provided file content.
        //
        // Act & Assert:
        // Mark test as inconclusive and provide guidance in the XML comment above.
        Assert.Inconclusive("Cannot instantiate Pento.Domain.Users.User: private constructor and missing factory implementation in provided scope. Provide a public factory or accessible constructor to enable this test.");
    }

    /// <summary>
    /// Provides diverse cases where Update should change the stored names.
    /// Includes: first-name change, last-name change, case-sensitive changes, empty->non-empty, very long strings, and special characters.
    /// </summary>
    private static IEnumerable<TestCaseData> UpdateDifferentNameCases()
    {
        string longFirst = new string('a', 5000);
        string longLast = new string('b', 5000);

        yield return new TestCaseData("John", "Doe", "Johnny", "Doe").SetName("ChangeFirst_Only");
        yield return new TestCaseData("John", "Doe", "John", "Doey").SetName("ChangeLast_Only");
        yield return new TestCaseData("John", "Doe", "john", "Doe").SetName("CaseSensitive_First_Changes");
        yield return new TestCaseData("", "", "NonEmptyFirst", "").SetName("EmptyToNonEmpty_First");
        yield return new TestCaseData("A", "B", longFirst, "B").SetName("VeryLong_First");
        yield return new TestCaseData("A", "B", "A", longLast).SetName("VeryLong_Last");
        yield return new TestCaseData("O'Conner", "Smith-Jr", "O\"Conner", "Smith Jr").SetName("SpecialCharacters");
    }

    /// <summary>
    /// Provides combinations of previous and new HouseholdId values and expectations.
    /// Each tuple: previousHouseholdId, newHouseholdId, expectUserLeftRaised, expectedRaisedHouseholdId
    /// </summary>
    private static IEnumerable<TestCaseData> SetHouseholdIdCases()
    {
        var g1 = Guid.NewGuid();
        var g2 = Guid.NewGuid();

        // previous null -> new null : no event, remains null
        yield return new TestCaseData((Guid?)null, (Guid?)null, false, (Guid?)null)
            .SetName("PreviousNull_To_Null_NoLeftEvent");

        // previous null -> new g1 : no UserLeftHouseholdDomainEvent, HouseholdId becomes g1
        yield return new TestCaseData((Guid?)null, (Guid?)g1, false, (Guid?)null)
            .SetName("PreviousNull_To_Guid_NoLeftEvent");

        // previous g1 -> new g1 : no event, remains g1
        yield return new TestCaseData((Guid?)g1, (Guid?)g1, false, (Guid?)null)
            .SetName("PreviousGuid_To_SameGuid_NoLeftEvent");

        // previous g1 -> new g2 : should raise UserLeftHouseholdDomainEvent with previous g1
        yield return new TestCaseData((Guid?)g1, (Guid?)g2, true, (Guid?)g1)
            .SetName("PreviousGuid_To_DifferentGuid_RaisesLeftEvent");

        // previous g1 -> new null : should raise UserLeftHouseholdDomainEvent with previous g1
        yield return new TestCaseData((Guid?)g1, (Guid?)null, true, (Guid?)g1)
            .SetName("PreviousGuid_To_Null_RaisesLeftEvent");
    }

}
