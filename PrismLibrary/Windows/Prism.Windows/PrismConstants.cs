// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

namespace Prism
{
    /// <summary>
    /// Internal class that contains the constants used by the Prism.Windows library.
    /// </summary>
    public static class PrismConstants
    {
        /// <summary>
        /// File name for session state data
        /// </summary>
        public const string SessionStateFileName = "_sessionState.xml";
        public const string FrameAlreadyRegisteredWithKey = "Frames can only be registered to one session state key";
        public const string FrameRegistrationRequirement = "Frames must be either be registered before accessing frame session state, or not registered at all";
        public const string SessionStateServiceFailed = "Session state service failed";
        public const string FrameNavigationServiceUnableResolveMessage = "Unable to resolve the pageToken {0} to a type.";
        public const string DefaultPageTypeLookupErrorMessage = "The page name {0} does not have an associated type in namespace {1}";
    }
}
