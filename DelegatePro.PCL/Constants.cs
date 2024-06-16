using System;
namespace DelegatePro.PCL
{
    public static class Constants
    {
        public const string NoInternetMessage = "No internet connection. Please connect and try again.";
        public const string LogoutMessage = "Are you sure you want to logout?";
        public const string LogoutMessageTitle = "Logout";
        public const string OfflineMessage = "Application is offline";

        public const string PhoneNumberFormat = "{0:(###) ###-####}";

        public const int SyncIntervalSeconds = 15;

        public static RGBWrapper OpenStatusColor = new RGBWrapper(70, 227, 110);
        public static RGBWrapper ClosedStatusColor = new RGBWrapper(255, 0, 0);
        public static RGBWrapper PlaceholderTextColor = new RGBWrapper(154, 154, 154);
        public static RGBWrapper SelectorTextColor = new RGBWrapper(68, 178, 235);
        public static RGBWrapper DefaultTextColor = new RGBWrapper(0, 0, 0);
        public static RGBWrapper BlueSelectionTextColor = new RGBWrapper(23, 83, 181);

        public const string SignOutIcon = "\xf08b";

        public static class Login
        {
            public const string LoginMessage = "Logging in...";
            public const string LoginErrorTitle = "Login";
            public const string UserNameAndPasswordRequiredMessage = "Username and Password are required.";
        }

        public static class LandingPage
        {
            public const string LoadingMessage = "Loading cases...";
            public const string ErrorTitle = "Cases";
            public const string ViewTitle = "Delegate Pro";
        }

        public static class CaseView
        {
            public const string ErrorTitle = "Case Error";
            public const string NewCaseTitle = "New Case";
            public const string IssueRequiredMessage = "Issue Description is required.";
            public const string SavingMessage = "Saving case...";
            public const string SavedMessageFormat = "Case saved";
            public const string NoEmployeesAdded = "Tap to add employees";
            public const string NoManagersAdded = "Tap to add managers";
            public const string NoUnionAdded = "Tap to add union delegates";
            public const string GrievanceStatusRequiredMessage = "Grievance Status is required.";
        }

        public static class ResetPassword
        {
            public const string ViewTitle = "Reset Password";
            public const string ErrorTitle = "Reset Password";
            public const string PasswordResetMessage = "Password reset email has been sent";
            public const string DialogMessage = "Resetting password...";
            public const string UserNameRequiredMessage = "Username is required to reset password";
            public const string ResetButtonText = "Reset";
        }

        public static class PersonDetail
        {
            public const string FirstNameLabelText = "First Name";
            public const string LastNameLabelText = "Last Name";
            public const string EmailLabelText = "Email";
            public const string CellPhoneTextLabel = "Cell Phone";
            public const string HomePhoneTextLabel = "Home Phone";
            public const string TypeTextLabel = "Type";
        }

        public static class POI
        {
            public const string FirstNameRequiredMessage = "First Name is required.";
            public const string LastNameRequiredMessage = "Last Name is required";
            public const string PersonTypeRequiredMessage = "Person Type is required";
            public const string ExistingPersonSelectionText = "Tap to choose";
            public const string ExistingPersonUnavailableText = "Unavailable during Edit mode";
            public const string RemovePersonFromCaseText = "Are you sure you want to remove this person from the case?";
            public const string RemovePersonFromOrgText = "Are you sure you want to remove this person from the organization?";
            public const string RemovePersonTitleText = "Remove Person?";
        }

        public static class Note
        {
            public const string TitleRequiredMessage = "Title is required.";
            public const string NoteTextRequiredMessage = "Note Text is required.";
            public const string ViewTitle = "Notes";
            public const string TitleText = "Title";
            public const string TextViewText = "Text";
            public const string AddressedText = "Addressed";
            public const string CaseLabelText = "Case";
            public const string VisibilityText = "Visibility";
            public const string ChildNoteText = "Parent Note";
            public const string NotAttachedToCaseMessage = "Select Case";
            public const string NoParentNote = "Select Parent Note";
            public const string CaseNotFound = "Case Not Found";
            public const string DeleteNoteTitleText = "Delete Note?";
            public const string DeleteNoteMessage = "Are you sure you want to delete this message?";
        }

        public static class Status
        {
            public const string FilterStatusViewTitle = "Status Filter";
            public const string ChooseStatusViewTitle = "Choose Status";
        }
    }
}

