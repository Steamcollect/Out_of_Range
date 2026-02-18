/// <summary>
/// Panel UI simple héritant de UI_PanelBase.
/// Utilisé pour les panels standalone qui ne font pas partie d'un système de tabs.
/// </summary>
public class UI_Panel : UI_PanelBase
{
    // Cette classe hérite maintenant de UI_PanelBase qui fournit:
    // - Gestion du CanvasGroup
    // - Animations FadeIn/FadeOut
    // - Liste de boutons avec events
    // - FirstSelected pour la navigation
    // - Events OnFocused/OnUnfocused
    // - Méthode TakeFocus() pour bloquer les interactions
    // - Méthode SetInteractable() pour activer/désactiver les boutons
    
    // Vous pouvez ajouter ici des fonctionnalités spécifiques à ce type de panel
}