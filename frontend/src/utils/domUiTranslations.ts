import { currentLanguage, Language, trLoose } from '../i18n';

const SKIP_TAGS = new Set(['SCRIPT', 'STYLE', 'TEXTAREA']);
const ATTRIBUTES = ['placeholder', 'title', 'aria-label'] as const;

const exactUiText: Record<string, Record<Language, string>> = {
  'profile': { en: 'Profile', fr: 'Profil', ar: 'الملف الشخصي' },
  'loading': { en: 'Loading...', fr: 'Chargement...', ar: 'جاري التحميل...' },
  'breadcrumb': { en: 'Breadcrumb', fr: 'Fil d’Ariane', ar: 'مسار الصفحة' },
  'upload profile image': { en: 'Upload profile image', fr: 'Téléverser l’image du profil', ar: 'رفع صورة الملف الشخصي' },
  'credentials': { en: 'Credentials', fr: 'Identifiants', ar: 'بيانات الدخول' },
  'to change password enter your current password and the new password leave both blank to change only the username': {
    en: 'To change password, enter your current password and the new password. Leave both blank to change only the username.',
    fr: 'Pour changer le mot de passe, saisissez l’ancien et le nouveau mot de passe. Laissez les deux champs vides pour changer seulement le nom utilisateur.',
    ar: 'لتغيير كلمة المرور، أدخل كلمة المرور الحالية والجديدة. اتركهما فارغين لتغيير اسم المستخدم فقط.',
  },
  'agency command center': { en: 'Agency command center', fr: 'Centre de contrôle agence', ar: 'مركز تحكم الوكالة' },
  'run balances services transactions cash controls reports and users from one clean agency operating system': {
    en: 'Run balances, services, transactions, cash controls, reports, and users from one clean agency operating system.',
    fr: 'Gérez soldes, services, transactions, caisse, rapports et utilisateurs depuis un seul système clair.',
    ar: 'سيّر الأرصدة والخدمات والمعاملات والصندوق والتقارير والمستخدمين من نظام واحد واضح.',
  },
  'authentication mode': { en: 'Authentication mode', fr: 'Mode d’authentification', ar: 'طريقة الدخول' },
  'sign in': { en: 'Sign in', fr: 'Connexion', ar: 'تسجيل الدخول' },
  'create agency': { en: 'Create agency', fr: 'Créer agence', ar: 'إنشاء وكالة' },
  'agency name': { en: 'Agency name', fr: 'Nom de l’agence', ar: 'اسم الوكالة' },
  'first name': { en: 'First name', fr: 'Prénom', ar: 'الاسم الشخصي' },
  'last name': { en: 'Last name', fr: 'Nom', ar: 'النسب' },
  'username': { en: 'Username', fr: 'Nom utilisateur', ar: 'اسم المستخدم' },
  'password': { en: 'Password', fr: 'Mot de passe', ar: 'كلمة المرور' },
  'light theme': { en: 'Light theme', fr: 'Thème clair', ar: 'الوضع الفاتح' },
  'dark theme': { en: 'Dark theme', fr: 'Thème sombre', ar: 'الوضع الداكن' },

  'service chart': { en: 'Service chart', fr: 'Graphe des services', ar: 'مبيان الخدمات' },
  'balance mise a jour instantanee': { en: 'Balance · Live update', fr: 'Solde · Mise à jour instantanée', ar: 'الرصيد · تحديث فوري' },
  'services': { en: 'Services', fr: 'Services', ar: 'الخدمات' },
  'service': { en: 'Service', fr: 'Service', ar: 'الخدمة' },
  'edit service': { en: 'Edit service', fr: 'Modifier service', ar: 'تعديل الخدمة' },
  'delete service': { en: 'Delete service', fr: 'Supprimer service', ar: 'حذف الخدمة' },
  'update service': { en: 'Update service', fr: 'Modifier service', ar: 'تعديل الخدمة' },
  'close': { en: 'Close', fr: 'Fermer', ar: 'إغلاق' },
  'save': { en: 'Save', fr: 'Enregistrer', ar: 'حفظ' },
  'saving': { en: 'Saving', fr: 'Enregistrement', ar: 'جاري الحفظ' },
  'cancel': { en: 'Cancel', fr: 'Annuler', ar: 'إلغاء' },
  'delete': { en: 'Delete', fr: 'Supprimer', ar: 'حذف' },
  'refresh': { en: 'Refresh', fr: 'Actualiser', ar: 'تحديث' },
  'select': { en: 'Select...', fr: 'Sélectionner...', ar: 'اختار...' },
  'selectionner': { en: 'Select...', fr: 'Sélectionner...', ar: 'اختار...' },
  'in': { en: 'In', fr: 'Entrée', ar: 'داخل' },
  'out': { en: 'Out', fr: 'Sortie', ar: 'خارج' },
  'in out': { en: 'In & out', fr: 'Entrée et sortie', ar: 'داخل وخارج' },

  'export pdf': { en: 'Export PDF', fr: 'Exporter PDF', ar: 'تصدير PDF' },
  'daily movement': { en: 'Daily movement', fr: 'Mouvement du jour', ar: 'حركة اليوم' },
  'high value': { en: 'High value', fr: 'Valeur élevée', ar: 'قيمة عالية' },
  'operations': { en: 'Operations', fr: 'Opérations', ar: 'العمليات' },
  'average ticket': { en: 'Average ticket', fr: 'Ticket moyen', ar: 'متوسط العملية' },
  'top service': { en: 'Top service', fr: 'Meilleur service', ar: 'أفضل خدمة' },
  'total balance': { en: 'Total balance', fr: 'Solde total', ar: 'الرصيد الإجمالي' },
  'services volume': { en: 'Services volume', fr: 'Volume services', ar: 'حجم الخدمات' },
  'transactions': { en: 'Transactions', fr: 'Transactions', ar: 'المعاملات' },

  'caisse': { en: 'Cash', fr: 'Caisse', ar: 'الصندوق' },
  'non paye details': { en: 'Unpaid details', fr: 'Détails non payés', ar: 'تفاصيل غير المدفوع' },
  'ajouter personne': { en: 'Add person', fr: 'Ajouter personne', ar: 'إضافة شخص' },
  'ajouter personne non paye': { en: 'Add unpaid person', fr: 'Ajouter personne non payée', ar: 'إضافة شخص غير مدفوع' },
  'nouvelle personne': { en: 'New person', fr: 'Nouvelle personne', ar: 'شخص جديد' },
  'nom': { en: 'Name', fr: 'Nom', ar: 'الاسم' },
  'ajouter': { en: 'Add', fr: 'Ajouter', ar: 'إضافة' },
  'add': { en: 'Add', fr: 'Ajouter', ar: 'إضافة' },
  'retrieve': { en: 'Retrieve', fr: 'Récupérer', ar: 'استرجاع' },
  'history': { en: 'History', fr: 'Historique', ar: 'السجل' },
  'ajouter non paye': { en: 'Add unpaid', fr: 'Ajouter non payé', ar: 'إضافة غير مدفوع' },
  'recuperer non paye': { en: 'Retrieve unpaid', fr: 'Récupérer non payé', ar: 'استرجاع غير مدفوع' },
  'ajouter montant': { en: 'Add amount', fr: 'Ajouter montant', ar: 'إضافة مبلغ' },
  'recuperer montant': { en: 'Retrieve amount', fr: 'Récupérer montant', ar: 'استرجاع مبلغ' },
  'historique non paye': { en: 'Unpaid history', fr: 'Historique non payé', ar: 'سجل غير المدفوع' },
  'aucun historique': { en: 'No history.', fr: 'Aucun historique.', ar: 'لا يوجد سجل.' },
  'aucun contributeur pour le compte cible': { en: 'No contributor for the target account.', fr: 'Aucun contributeur pour le compte cible.', ar: 'لا يوجد مساهم لهذا الحساب.' },

  'register clients': { en: 'Register clients', fr: 'Registre clients', ar: 'سجل الزبناء' },
  'search client info document': { en: 'Search client, info, document...', fr: 'Rechercher client, info, document...', ar: 'بحث عن زبون أو معلومة أو وثيقة...' },
  'new client': { en: 'New client', fr: 'Nouveau client', ar: 'زبون جديد' },
  'save client': { en: 'Save client', fr: 'Enregistrer client', ar: 'حفظ الزبون' },
  'no documents': { en: 'No documents', fr: 'Aucun document', ar: 'لا توجد وثائق' },
  'no clients found': { en: 'No clients found.', fr: 'Aucun client trouvé.', ar: 'لا يوجد زبناء.' },

  'utilisateurs': { en: 'Users', fr: 'Utilisateurs', ar: 'المستخدمون' },
  'utilisateur': { en: 'User', fr: 'Utilisateur', ar: 'المستخدم' },
  'new user': { en: 'New user', fr: 'Nouvel utilisateur', ar: 'مستخدم جديد' },
  'leave blank to keep': { en: 'Leave blank to keep', fr: 'Laisser vide pour garder', ar: 'اتركه فارغا للإبقاء عليه' },
  'active': { en: 'Active', fr: 'Actif', ar: 'نشط' },
  'user permissions': { en: 'User permissions', fr: 'Permissions utilisateur', ar: 'صلاحيات المستخدم' },
  'section': { en: 'Section', fr: 'Section', ar: 'القسم' },
  'save user': { en: 'Save user', fr: 'Enregistrer utilisateur', ar: 'حفظ المستخدم' },

  'expenses income': { en: 'Expenses & income', fr: 'Dépenses et revenus', ar: 'المصاريف والمداخيل' },
  'income': { en: 'Income', fr: 'Revenus', ar: 'المداخيل' },
  'expenses': { en: 'Expenses', fr: 'Dépenses', ar: 'المصاريف' },
  'net': { en: 'Net', fr: 'Net', ar: 'الصافي' },
  'today net': { en: 'Today net', fr: 'Net du jour', ar: 'صافي اليوم' },
  'recent entries': { en: 'Recent entries', fr: 'Dernières entrées', ar: 'آخر العمليات' },
  'no entries yet': { en: 'No entries yet.', fr: 'Aucune entrée.', ar: 'لا توجد عمليات بعد.' },

  'blocks': { en: 'Blocks', fr: 'Blocs', ar: 'الكتل' },
  'inspector': { en: 'Inspector', fr: 'Inspecteur', ar: 'المراقب' },
  'name': { en: 'Name', fr: 'Nom', ar: 'الاسم' },
  'description': { en: 'Description', fr: 'Description', ar: 'الوصف' },
  'select a node': { en: 'Select a node', fr: 'Sélectionner un nœud', ar: 'اختر عنصرا' },
  'transaction workflows': { en: 'Transaction workflows', fr: 'Workflows transactions', ar: 'مسارات المعاملات' },
  'each service uses its saved type to decide which directions can be configured then each direction chooses its own source and target accounts': {
    en: 'Each service uses its saved type to decide which directions can be configured, then each direction chooses its own source and target accounts.',
    fr: 'Chaque service utilise son type enregistré pour définir les directions configurables, puis chaque direction choisit ses comptes source et destination.',
    ar: 'كل خدمة تستعمل نوعها المحفوظ لتحديد الاتجاهات الممكنة، ثم يختار كل اتجاه حساب المصدر والوجهة.',
  },
  'no services yet': { en: 'No services yet', fr: 'Aucun service', ar: 'لا توجد خدمات' },
  'service selected': { en: 'Service selected', fr: 'Service sélectionné', ar: 'تم اختيار الخدمة' },
  'direction checked': { en: 'Direction checked', fr: 'Direction vérifiée', ar: 'تم فحص الاتجاه' },
  'transfer saved': { en: 'Transfer saved', fr: 'Transfert enregistré', ar: 'تم حفظ التحويل' },
  'account actions and history saved': { en: 'Account actions and history saved', fr: 'Actions et historique compte enregistrés', ar: 'تم حفظ إجراءات الحساب والسجل' },
  'create a service first': { en: 'Create a service first.', fr: 'Créer un service d’abord.', ar: 'أنشئ خدمة أولا.' },
  'actions des comptes': { en: 'Account actions', fr: 'Actions des comptes', ar: 'إجراءات الحسابات' },
  'configure les evenements executes sur chaque transaction quand l argent entre ou sort d un compte la meme somme peut etre ajoutee ou retiree aux comptes lies': {
    en: 'Configure events executed on each transaction. When money enters or leaves an account, the same amount can be added to or removed from linked accounts.',
    fr: 'Configure les événements exécutés sur chaque transaction. Quand l’argent entre ou sort d’un compte, la même somme peut être ajoutée ou retirée aux comptes liés.',
    ar: 'اضبط الأحداث المنفذة في كل معاملة. عند دخول أو خروج المال من حساب، يمكن إضافة أو خصم نفس المبلغ من الحسابات المرتبطة.',
  },
  'nouvelle action': { en: 'New action', fr: 'Nouvelle action', ar: 'إجراء جديد' },
  'aucune action configuree': { en: 'No action configured.', fr: 'Aucune action configurée.', ar: 'لا يوجد إجراء مضبوط.' },
  'action active': { en: 'Active action', fr: 'Action active', ar: 'إجراء نشط' },
  'argent entre': { en: 'Money in', fr: 'Argent entre', ar: 'دخول المال' },
  'argent sort': { en: 'Money out', fr: 'Argent sort', ar: 'خروج المال' },
  'ajouter le montant': { en: 'Add amount', fr: 'Ajouter le montant', ar: 'إضافة المبلغ' },
  'retirer le montant': { en: 'Subtract amount', fr: 'Retirer le montant', ar: 'خصم المبلغ' },
  'comptes lies': { en: 'Linked accounts', fr: 'Comptes liés', ar: 'الحسابات المرتبطة' },
  'cree une action pour commencer': { en: 'Create an action to start.', fr: 'Crée une action pour commencer.', ar: 'أنشئ إجراء للبدء.' },
  'execution': { en: 'Execution', fr: 'Exécution', ar: 'التنفيذ' },
  'selectionne une action': { en: 'Select an action.', fr: 'Sélectionne une action.', ar: 'اختر إجراء.' },

  'back to services': { en: 'Back to services', fr: 'Retour services', ar: 'الرجوع للخدمات' },
  'refresh comptes': { en: 'Refresh accounts', fr: 'Actualiser comptes', ar: 'تحديث الحسابات' },
  'lie au comptes': { en: 'Linked to accounts:', fr: 'Lié aux comptes :', ar: 'مرتبط بالحسابات:' },
  'check': { en: 'Check', fr: 'Vérifier', ar: 'تحقق' },
  'upload excel or csv review the scanned table then save the rows': {
    en: 'Upload Excel or CSV, review the scanned table, then save the rows.',
    fr: 'Importez Excel ou CSV, vérifiez le tableau scanné, puis enregistrez les lignes.',
    ar: 'ارفع ملف Excel أو CSV، راجع الجدول، ثم احفظ الأسطر.',
  },
  'ai scan': { en: 'AI scan', fr: 'Scan IA', ar: 'مسح بالذكاء الاصطناعي' },
  'manual rules': { en: 'Manual rules', fr: 'Règles manuelles', ar: 'قواعد يدوية' },
  'kind': { en: 'Kind', fr: 'Genre', ar: 'النوع' },
  'type': { en: 'Type', fr: 'Type', ar: 'الصنف' },
  'fee': { en: 'Fee', fr: 'Frais', ar: 'الرسوم' },
  'solde': { en: 'Balance', fr: 'Solde', ar: 'الرصيد' },
  'date time': { en: 'Date/time', fr: 'Date/heure', ar: 'التاريخ/الوقت' },
  'status': { en: 'Status', fr: 'Statut', ar: 'الحالة' },
  'no import loaded': { en: 'No import loaded', fr: 'Aucun import chargé', ar: 'لا يوجد ملف مستورد' },
  'upload an excel csv file': { en: 'Upload an Excel/CSV file.', fr: 'Importez un fichier Excel/CSV.', ar: 'ارفع ملف Excel/CSV.' },
  'manual transaction': { en: 'Manual transaction', fr: 'Transaction manuelle', ar: 'معاملة يدوية' },
  'id': { en: 'ID', fr: 'ID', ar: 'المعرف' },
  'frais': { en: 'Fees', fr: 'Frais', ar: 'الرسوم' },
  'etat': { en: 'State', fr: 'État', ar: 'الحالة' },

  'settings': { en: 'Settings', fr: 'Paramètres', ar: 'الإعدادات' },
  'parametres generaux': { en: 'General settings', fr: 'Paramètres généraux', ar: 'الإعدادات العامة' },
  'choisir le compte qui recoit le total calcule dans la section caisse': {
    en: 'Choose the account that receives the calculated total in the cash section.',
    fr: 'Choisir le compte qui reçoit le total calculé dans la section caisse.',
    ar: 'اختر الحساب الذي يستقبل المجموع المحسوب في قسم الصندوق.',
  },
  'compte caisse': { en: 'Cash account', fr: 'Compte caisse', ar: 'حساب الصندوق' },
  'compte non paye': { en: 'Unpaid account', fr: 'Compte non payé', ar: 'حساب غير المدفوع' },
  'aucun compte': { en: 'No account', fr: 'Aucun compte', ar: 'لا يوجد حساب' },
  'excel import': { en: 'Excel import', fr: 'Import Excel', ar: 'استيراد Excel' },
  'default import mode': { en: 'Default import mode', fr: 'Mode d’import par défaut', ar: 'طريقة الاستيراد الافتراضية' },
  'ai provider': { en: 'AI provider', fr: 'Fournisseur IA', ar: 'مزود الذكاء الاصطناعي' },
  'openai model': { en: 'OpenAI model', fr: 'Modèle OpenAI', ar: 'نموذج OpenAI' },
  'openai api key': { en: 'OpenAI API key', fr: 'Clé API OpenAI', ar: 'مفتاح OpenAI API' },
  'gemini model': { en: 'Gemini model', fr: 'Modèle Gemini', ar: 'نموذج Gemini' },
  'gemini api key': { en: 'Gemini API key', fr: 'Clé API Gemini', ar: 'مفتاح Gemini API' },
  'use env key when empty': { en: 'Use .env key when empty', fr: 'Utiliser la clé .env si vide', ar: 'استعمل مفتاح .env إذا كان فارغا' },
  'prompt configuration': { en: 'Prompt configuration', fr: 'Configuration du prompt', ar: 'إعدادات التوجيه' },
  'manual service detection': { en: 'Manual service detection', fr: 'Détection manuelle du service', ar: 'كشف الخدمة يدويا' },
  'add rule': { en: 'Add rule', fr: 'Ajouter règle', ar: 'إضافة قاعدة' },
  'enabled': { en: 'Enabled', fr: 'Activé', ar: 'مفعل' },
  'rule name': { en: 'Rule name', fr: 'Nom de la règle', ar: 'اسم القاعدة' },
  'example dmane cash withdrawals': { en: 'Example: Dmane cash withdrawals', fr: 'Exemple : retraits cash Dmane', ar: 'مثال: سحوبات كاش Dmane' },
  'match condition': { en: 'Match condition', fr: 'Condition de recherche', ar: 'شرط المطابقة' },
  'starts with': { en: 'Starts with', fr: 'Commence par', ar: 'يبدأ بـ' },
  'equals': { en: 'Equals', fr: 'Égale', ar: 'يساوي' },
  'contains': { en: 'Contains', fr: 'Contient', ar: 'يحتوي على' },
  'ends with': { en: 'Ends with', fr: 'Se termine par', ar: 'ينتهي بـ' },
  'regex': { en: 'Regex', fr: 'Regex', ar: 'Regex' },
  'description text': { en: 'Description text', fr: 'Texte description', ar: 'نص الوصف' },
  'text to find for example cash out or ddd': { en: 'Text to find, for example: CASH OUT or ddd', fr: 'Texte à chercher, exemple : CASH OUT ou ddd', ar: 'النص المراد البحث عنه، مثال: CASH OUT أو ddd' },
  'detected service': { en: 'Detected service', fr: 'Service détecté', ar: 'الخدمة المكتشفة' },
  'choose the service for matched rows': { en: 'Choose the service for matched rows', fr: 'Choisir le service pour les lignes détectées', ar: 'اختر الخدمة للأسطر المطابقة' },
  'transaction type': { en: 'Transaction type', fr: 'Type de transaction', ar: 'نوع المعاملة' },
  'auto from file or service': { en: 'Auto from file or service', fr: 'Auto depuis fichier ou service', ar: 'تلقائي من الملف أو الخدمة' },
  'case sensitive distinguish uppercase and lowercase text': { en: 'Case sensitive: distinguish uppercase and lowercase text', fr: 'Sensible à la casse : distingue majuscules et minuscules', ar: 'حساس لحالة الأحرف: يفرق بين الكبير والصغير' },
  'delete rule': { en: 'Delete rule', fr: 'Supprimer règle', ar: 'حذف القاعدة' },
  'no manual import rules yet': { en: 'No manual import rules yet.', fr: 'Aucune règle manuelle.', ar: 'لا توجد قواعد يدوية بعد.' },
  'validities permissions': { en: 'Validity permissions', fr: 'Permissions des validations', ar: 'صلاحيات التحقق' },
  'settings saved': { en: 'Settings saved', fr: 'Paramètres enregistrés', ar: 'تم حفظ الإعدادات' },
  'save settings': { en: 'Save settings', fr: 'Enregistrer paramètres', ar: 'حفظ الإعدادات' },

  'configuration des cartes comptes': { en: 'Account card configuration', fr: 'Configuration des cartes comptes', ar: 'إعدادات بطاقات الحسابات' },
  'aucun compte disponible': { en: 'No account available.', fr: 'Aucun compte disponible.', ar: 'لا يوجد حساب متاح.' },
  'configuration dynamique des comptes': { en: 'Dynamic account configuration', fr: 'Configuration dynamique des comptes', ar: 'إعدادات الحسابات الديناميكية' },
  'compte': { en: 'Account', fr: 'Compte', ar: 'الحساب' },
  'use variables between braces for example ancien solde or bank each account name is available as a variable automatically': {
    en: 'Use variables between braces, for example {Ancien solde} or {Bank}. Each account name is available as a variable automatically.',
    fr: 'Utilisez des variables entre accolades, par exemple {Ancien solde} ou {Bank}. Chaque nom de compte est disponible automatiquement.',
    ar: 'استعمل المتغيرات بين الأقواس، مثلا {Ancien solde} أو {Bank}. كل اسم حساب متاح كمتغير تلقائيا.',
  },
  'account visibility': { en: 'Account visibility', fr: 'Visibilité du compte', ar: 'رؤية الحساب' },
  'admin always sees every account selected roles or users will not see this account its balance or its history': {
    en: 'Admin always sees every account. Selected roles or users will not see this account, its balance, or its history.',
    fr: 'L’admin voit toujours tous les comptes. Les rôles ou utilisateurs sélectionnés ne verront pas ce compte, son solde ou son historique.',
    ar: 'المدير يرى كل الحسابات دائما. الأدوار أو المستخدمون المختارون لن يروا هذا الحساب أو رصيده أو سجله.',
  },
  'hide from roles': { en: 'Hide from roles', fr: 'Masquer pour les rôles', ar: 'إخفاء عن الأدوار' },
  'hide from users': { en: 'Hide from users', fr: 'Masquer pour les utilisateurs', ar: 'إخفاء عن المستخدمين' },
  'no non admin users found': { en: 'No non-admin users found.', fr: 'Aucun utilisateur non-admin.', ar: 'لا يوجد مستخدمون غير المدير.' },
  'text fields': { en: 'Text fields', fr: 'Champs texte', ar: 'حقول النص' },
  'add text': { en: 'Add text', fr: 'Ajouter texte', ar: 'إضافة نص' },
  'show': { en: 'Show', fr: 'Afficher', ar: 'إظهار' },
  'label': { en: 'Label', fr: 'Libellé', ar: 'التسمية' },
  'formula text': { en: 'Formula / text', fr: 'Formule / texte', ar: 'صيغة / نص' },
  'pos': { en: 'Pos', fr: 'Pos', ar: 'الموضع' },
  'popup buttons': { en: 'Popup buttons', fr: 'Boutons popup', ar: 'أزرار النافذة' },
  'add button': { en: 'Add button', fr: 'Ajouter bouton', ar: 'إضافة زر' },
  'these buttons and actions are saved only for the selected compte': {
    en: 'These buttons and actions are saved only for the selected account.',
    fr: 'Ces boutons et actions sont enregistrés seulement pour le compte sélectionné.',
    ar: 'هذه الأزرار والإجراءات تحفظ فقط للحساب المختار.',
  },
  'popup action': { en: 'Popup / action', fr: 'Popup / action', ar: 'نافذة / إجراء' },
  'popup config saved': { en: 'Popup config saved', fr: 'Configuration popup enregistrée', ar: 'تم حفظ إعدادات النافذة' },
  'reset popups': { en: 'Reset popups', fr: 'Réinitialiser popups', ar: 'إعادة ضبط النوافذ' },
  'save popups': { en: 'Save popups', fr: 'Enregistrer popups', ar: 'حفظ النوافذ' },
  'versement retrait popup': { en: 'Deposit / withdrawal popup', fr: 'Popup versement / retrait', ar: 'نافذة الإيداع / السحب' },
  'popup title': { en: 'Popup title', fr: 'Titre popup', ar: 'عنوان النافذة' },
  'default operation': { en: 'Default operation', fr: 'Opération par défaut', ar: 'العملية الافتراضية' },
  'versement': { en: 'Deposit', fr: 'Versement', ar: 'إيداع' },
  'retrait': { en: 'Withdrawal', fr: 'Retrait', ar: 'سحب' },
  'apply fixed operation type': { en: 'Apply fixed operation type', fr: 'Appliquer un type fixe', ar: 'استعمال نوع عملية ثابت' },
  'fixed operation type': { en: 'Fixed operation type', fr: 'Type d’opération fixe', ar: 'نوع العملية الثابت' },
  'versement toggle label': { en: 'Deposit toggle label', fr: 'Libellé bouton versement', ar: 'تسمية زر الإيداع' },
  'retrait toggle label': { en: 'Withdrawal toggle label', fr: 'Libellé bouton retrait', ar: 'تسمية زر السحب' },
  'account label': { en: 'Account label', fr: 'Libellé compte', ar: 'تسمية الحساب' },
  'apply fixed compte': { en: 'Apply fixed account', fr: 'Appliquer un compte fixe', ar: 'استعمال حساب ثابت' },
  'fixed compte': { en: 'Fixed account', fr: 'Compte fixe', ar: 'الحساب الثابت' },
  'amount label': { en: 'Amount label', fr: 'Libellé montant', ar: 'تسمية المبلغ' },
  'apply fixed amount': { en: 'Apply fixed amount', fr: 'Appliquer un montant fixe', ar: 'استعمال مبلغ ثابت' },
  'fixed amount': { en: 'Fixed amount', fr: 'Montant fixe', ar: 'المبلغ الثابت' },
  'validate label': { en: 'Validate label', fr: 'Libellé validation', ar: 'تسمية التأكيد' },
  'cancel label': { en: 'Cancel label', fr: 'Libellé annulation', ar: 'تسمية الإلغاء' },
  'description label': { en: 'Description label', fr: 'Libellé description', ar: 'تسمية الوصف' },
  'show description field': { en: 'Show description field', fr: 'Afficher le champ description', ar: 'إظهار حقل الوصف' },
  'apply fixed description': { en: 'Apply fixed description', fr: 'Appliquer une description fixe', ar: 'استعمال وصف ثابت' },
  'fixed description': { en: 'Fixed description', fr: 'Description fixe', ar: 'الوصف الثابت' },
  'show contributor name in versement retrait': { en: 'Show contributor name in deposit/withdrawal', fr: 'Afficher le nom contributeur en versement/retrait', ar: 'إظهار اسم المساهم في الإيداع/السحب' },
  'contributor label': { en: 'Contributor label', fr: 'Libellé contributeur', ar: 'تسمية المساهم' },
  'transfert popup': { en: 'Transfer popup', fr: 'Popup transfert', ar: 'نافذة التحويل' },
  'from label': { en: 'From label', fr: 'Libellé source', ar: 'تسمية المصدر' },
  'apply fixed from compte': { en: 'Apply fixed source account', fr: 'Appliquer compte source fixe', ar: 'استعمال حساب مصدر ثابت' },
  'fixed from compte': { en: 'Fixed source account', fr: 'Compte source fixe', ar: 'حساب المصدر الثابت' },
  'to label': { en: 'To label', fr: 'Libellé destination', ar: 'تسمية الوجهة' },
  'apply fixed to compte': { en: 'Apply fixed target account', fr: 'Appliquer compte destination fixe', ar: 'استعمال حساب وجهة ثابت' },
  'fixed to compte': { en: 'Fixed target account', fr: 'Compte destination fixe', ar: 'حساب الوجهة الثابت' },
  'configuration du compte sauvegardee': { en: 'Account configuration saved', fr: 'Configuration du compte sauvegardée', ar: 'تم حفظ إعدادات الحساب' },
  'reset compte': { en: 'Reset account', fr: 'Réinitialiser compte', ar: 'إعادة ضبط الحساب' },
  'save compte': { en: 'Save account', fr: 'Enregistrer compte', ar: 'حفظ الحساب' },
};

function normalizeText(value: string) {
  return value
    .toLowerCase()
    .normalize('NFD')
    .replace(/[\u0300-\u036f]/g, '')
    .replace(/[^\p{L}\p{N}]+/gu, ' ')
    .trim();
}

function translateText(value: string) {
  const trimmed = value.trim();
  if (!trimmed) return value;
  const memory = exactUiText[normalizeText(trimmed)];
  const translated = memory?.[currentLanguage()] ?? trLoose(trimmed);
  return translated === trimmed ? value : value.replace(trimmed, translated);
}

function translateElement(element: Element) {
  if (SKIP_TAGS.has(element.tagName)) return;
  ATTRIBUTES.forEach((attribute) => {
    const value = element.getAttribute(attribute);
    if (!value) return;
    const translated = translateText(value);
    if (translated !== value) element.setAttribute(attribute, translated);
  });
}

function translateNode(node: Node) {
  if (node.nodeType === Node.TEXT_NODE) {
    const parent = node.parentElement;
    if (!parent || SKIP_TAGS.has(parent.tagName)) return;
    const translated = translateText(node.textContent ?? '');
    if (translated !== node.textContent) node.textContent = translated;
    return;
  }

  if (node.nodeType !== Node.ELEMENT_NODE) return;
  const element = node as Element;
  translateElement(element);
  element.querySelectorAll('*').forEach(translateElement);
  const walker = document.createTreeWalker(element, NodeFilter.SHOW_TEXT);
  const textNodes: Node[] = [];
  while (walker.nextNode()) textNodes.push(walker.currentNode);
  textNodes.forEach(translateNode);
}

export function translateStaticUi(root: ParentNode = document.body) {
  root.childNodes.forEach(translateNode);
}

export function installStaticUiTranslator() {
  if (typeof document === 'undefined') return () => undefined;
  translateStaticUi();
  const observer = new MutationObserver((mutations) => {
    mutations.forEach((mutation) => {
      if (mutation.type === 'attributes') {
        translateElement(mutation.target as Element);
        return;
      }
      mutation.addedNodes.forEach(translateNode);
      if (mutation.type === 'characterData') translateNode(mutation.target);
    });
  });
  observer.observe(document.body, {
    attributes: true,
    attributeFilter: [...ATTRIBUTES],
    childList: true,
    characterData: true,
    subtree: true,
  });
  return () => observer.disconnect();
}
