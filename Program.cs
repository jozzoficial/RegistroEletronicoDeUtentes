namespace RegistroEletronicoDeUtentes
{

    //Vou deixar comentarios explicativos no codigopara melhor entendimento do fluxo
    public class Program
    {
        // Defini nesse escopo a capacidade máxima de utentes e consultas permitidas no sistema
        const int MAX_UTENTES = 100;
        const int MAX_CONSULTAS = 500;

        // Dados dos pacientes
        static string[] numeroRegisto = new string[MAX_UTENTES];
        static string[] nome = new string[MAX_UTENTES];
        static string[] dataNascimento = new string[MAX_UTENTES];
        static string[] sexo = new string[MAX_UTENTES];
        static string[] telefone = new string[MAX_UTENTES];
        static string[] morada = new string[MAX_UTENTES];
        static string[] nif = new string[MAX_UTENTES];
        static int totalUtentes = 0;

        // Dados das consultas
        static string[] consultaUtente = new string[MAX_CONSULTAS];
        static string[] consultaData = new string[MAX_CONSULTAS];
        static string[] consultaProfissional = new string[MAX_CONSULTAS];
        static string[] consultaDiagnostico = new string[MAX_CONSULTAS];
        static string[] consultaNotas = new string[MAX_CONSULTAS];
        static int totalConsultas = 0;

        // Users do sistema (Nível 1 = Recepção, nível 2 = Médico, nível 3 = Enfermeiro)
        static string[] loginUsuarios = { "recepcao", "medico", "enfermeiro" };
        static string[] senhaUsuarios = { "1234", "1234", "1234" };
        static string[] nomeUsuarios = { "Recepcionista", "Dr. Médico", "Enfermeiro" };
        static int[] nivelUsuarios = { 1, 2, 3 };

        // User com sessão iniciada
        static string profissionalAtual = "";
        static int nivelAtual = 0;

        //Menu
        static void MostrarMenu()
        {
            Console.WriteLine();
            Console.WriteLine("------------- MENU PRINCIPAL -------------");
            Console.WriteLine($"Utilizador: {profissionalAtual}");
            Console.WriteLine("1. Registar novo utente");
            Console.WriteLine("2. Listar utentes");
            Console.WriteLine("3. Pesquisar utente (Nº de registo ou NIF)");
            Console.WriteLine("4. Registar consulta");
            Console.WriteLine("5. Consultar histórico clínico");
            Console.WriteLine("0. Sair");
            Console.WriteLine("------------------------------------------");
        }


        // Funçao para fazer auteticação e dar permissões ao usuario
        static bool FazerLogin()
        {
            int tentativas = 0;

            while (tentativas < 3)
            {
                Console.Write("User: ");
                string login = (Console.ReadLine() ?? "").Trim().ToLower();
                Console.Write("Passe: ");
                string senha = (Console.ReadLine() ?? "").Trim();

                for (int i = 0; i < loginUsuarios.Length; i++)
                {
                    if (loginUsuarios[i] == login && senhaUsuarios[i] == senha)
                    {
                        profissionalAtual = nomeUsuarios[i];
                        nivelAtual = nivelUsuarios[i];
                        Console.WriteLine($"Bem-vindo, {profissionalAtual},");
                        return true;
                    }
                }
                tentativas++;
                Console.WriteLine($"Utilizador ou senha incorretos. Tentativa {tentativas} de 3.");
            }
            return false;
        }

        //Verificar se o nível do utilizador permite executar a opção do menu
        static bool TemPermissao(int opcao)
        {
            bool permitido = false;

            if (opcao == 1)
            {
                // Registar utente: recepção e médico
                permitido = (nivelAtual == 1 || nivelAtual == 2);
            }
            else if (opcao == 4)
            {
                // Registar consulta: apenas médico
                permitido = (nivelAtual == 2);
            }
            else if (opcao == 5)
            {
                // Consultar histórico clínico: médico e enfermeiro
                permitido = (nivelAtual == 2 || nivelAtual == 3);
            }

            if (!permitido)
            {
                Console.WriteLine("Acesso negado! o seu perfil não tem autorização para esta operação.");
            }
            return permitido;
        }


        //Função para registrar utente
        static void RegistarUtente()
        {
            if (totalUtentes >= MAX_UTENTES)
            {
                Console.WriteLine("Capacidade máxima de utentes atingida.");
                return;
            }

            Console.Clear();
            Console.WriteLine();
            Console.WriteLine("--- Registo de novo utente ---");

            string nomeCompleto = LerTextoObrigatorio("Nome completo: ");
            string nascimento = LerData("Data de nascimento (dd/mm/aaaa): ");
            string sexoUtente = LerSexo();
            string tel = LerTextoObrigatorio("Telefone: ");
            string enderecoUtente = LerTextoObrigatorio("Morada: ");

            // O NIF é associado apenas quando aplicável
            string nifUtente = "";
            Console.Write("Associar NIF a este registo? (S/N): ");
            string resposta = (Console.ReadLine() ?? "").Trim().ToUpper();

            if (resposta == "S")
            {
                nifUtente = LerTextoObrigatorio("NIF: ").ToUpper();

                if (ProcurarPorNif(nifUtente) != -1)
                {
                    Console.WriteLine("Já existe um utente com esse NIF. Registo cancelado.");
                    return;
                }
            }

            // Número único de registo: UT0001, UT0002, etc..
            string numero = "UT" + (totalUtentes + 1).ToString("0000");

            numeroRegisto[totalUtentes] = numero;
            nome[totalUtentes] = nomeCompleto;
            dataNascimento[totalUtentes] = nascimento;
            sexo[totalUtentes] = sexoUtente;
            telefone[totalUtentes] = tel;
            morada[totalUtentes] = enderecoUtente;
            nif[totalUtentes] = nifUtente;
            totalUtentes++;

            Console.WriteLine("Utente registado com sucesso. Número de registo: " + numero);
        }


        //Função para listar utentes
        static void ListarUtentes()
        {
            Console.Clear();
            Console.WriteLine();
            Console.WriteLine("--- Lista de utentes ---");

            if (totalUtentes == 0)
            {
                Console.WriteLine("Ainda não há utentes registados.");
                return;
            }

            for (int i = 0; i < totalUtentes; i++)
            {
                Console.WriteLine($"{numeroRegisto[i]} | {nome[i]} | {dataNascimento[i]}");
            }

            Console.WriteLine($"Total: {totalUtentes} utente(s).");
        }


        //Função para pesquiser utentes regisrrados
        static void PesquisarUtente()
        {
            Console.Clear();
            Console.WriteLine();
            Console.WriteLine("--- Pesquisa de utente ---");
            Console.WriteLine("1 - Por número de registo");
            Console.WriteLine("2 - Por NIF");
            int tipo = LerInteiro("Opção: ");

            int posicao = -1;

            if (tipo == 1)
            {
                string numero = LerTextoObrigatorio("Número de registo: ").ToUpper();
                posicao = ProcurarPorNumero(numero);
            }
            else if (tipo == 2)
            {
                string numeroNif = LerTextoObrigatorio("NIF: ").ToUpper();
                posicao = ProcurarPorNif(numeroNif);
            }
            else
            {
                Console.WriteLine("Opção inválida.");
                return;
            }

            if (posicao == -1)
            {
                Console.WriteLine("Utente não encontrado.");
            }
            else
            {
                MostrarFicha(posicao);
            }
        }


        //Função para registrar nova Consultas
        static void RegistarConsulta()
        {
            if (totalConsultas >= MAX_CONSULTAS)
            {
                Console.WriteLine("Capacidade máxima de consultas atingida.");
                return;
            }
            Console.Clear();
            Console.WriteLine();
            Console.WriteLine("--- Registo de consulta ---");

            string numero = LerTextoObrigatorio("Número de registo do utente: ").ToUpper();
            int posicao = ProcurarPorNumero(numero);

            if (posicao == -1)
            {
                Console.WriteLine("Utente não encontrado.");
                return;
            }
            Console.WriteLine("Utente: " + nome[posicao]);

            string diagnostico = LerTextoObrigatorio("Diagnóstico: ");
            string notas = LerTextoObrigatorio("Outras informações clínicas: ");

            consultaUtente[totalConsultas] = numero;
            consultaData[totalConsultas] = DateTime.Now.ToString("dd/MM/yyyy");
            consultaProfissional[totalConsultas] = profissionalAtual;
            consultaDiagnostico[totalConsultas] = diagnostico;
            consultaNotas[totalConsultas] = notas;
            totalConsultas++;

            Console.WriteLine("Consulta registada com sucesso.");
        }


        //Função para ver histórico
        static void VerHistorico()
        {
            Console.Clear();
            Console.WriteLine();
            Console.WriteLine("--- Histórico clínico ---");

            string numero = LerTextoObrigatorio("Número de registo do utente: ").ToUpper();
            int posicao = ProcurarPorNumero(numero);

            if (posicao == -1)
            {
                Console.WriteLine("Utente não encontrado.");
                return;
            }

            MostrarFicha(posicao);
            Console.WriteLine();
            Console.WriteLine("Consultas realizadas:");

            int encontradas = 0;

            for (int i = 0; i < totalConsultas; i++)
            {
                if (consultaUtente[i] == numero)
                {
                    encontradas++;
                    Console.WriteLine();
                    Console.WriteLine("Consulta " + encontradas);
                    Console.WriteLine("  Data de atendimento: " + consultaData[i]);
                    Console.WriteLine("  Profissional: " + consultaProfissional[i]);
                    Console.WriteLine("  Diagnóstico: " + consultaDiagnostico[i]);
                    Console.WriteLine("  Informações clínicas: " + consultaNotas[i]);
                }
            }

            if (encontradas == 0)
            {
                Console.WriteLine("Este utente ainda não tem consultas registadas.");
            }
        }

        //Akgumas funções auxiliares
        static int ProcurarPorNumero(string numero)
        {
            for (int i = 0; i < totalUtentes; i++)
            {
                if (numeroRegisto[i] == numero) return i;
            }
            return -1;
        }

        static int ProcurarPorNif(string numeroNif)
        {
            for (int i = 0; i < totalUtentes; i++)
            {
                if (nif[i] != "" && nif[i] == numeroNif) return i;
            }
            return -1;
        }

        static void MostrarFicha(int i)
        {
            Console.WriteLine("Nº de registo: " + numeroRegisto[i]);
            Console.WriteLine("Nome: " + nome[i]);
            Console.WriteLine("Data de nascimento: " + dataNascimento[i]);
            Console.WriteLine("Sexo: " + sexo[i]);
            Console.WriteLine("Telefone: " + telefone[i]);
            Console.WriteLine("Morada: " + morada[i]);

            if (nif[i] == "")
                Console.WriteLine("NIF: não associado");
            else
                Console.WriteLine("NIF: " + nif[i]);
        }

        //Funções de leitura com validação
        static string LerTextoObrigatorio(string mensagem)
        {
            string texto = "";

            while (texto == "")
            {
                Console.Write(mensagem);
                texto = (Console.ReadLine() ?? "").Trim();

                if (texto == "")
                {
                    Console.WriteLine("Este campo não pode ficar vazio.");
                }
            }

            return texto;
        }

        static int LerInteiro(string mensagem)
        {
            int valor;

            Console.Write(mensagem);
            string texto = Console.ReadLine() ?? "";

            if (int.TryParse(texto, out valor))
                return valor;

            return -1;
        }

        static string LerSexo()
        {
            while (true)
            {
                Console.Write("Sexo (M/F): ");
                string texto = (Console.ReadLine() ?? "").Trim().ToUpper();

                if (texto == "M") return "Masculino";
                if (texto == "F") return "Feminino";

                Console.WriteLine("Digite M ou F.");
            }
        }

        static string LerData(string mensagem)
        {
            while (true)
            {
                Console.Write(mensagem);
                string texto = (Console.ReadLine() ?? "").Trim();

                if (DataValida(texto)) return texto;

                Console.WriteLine("Data inválida. Use o formato dd/mm/aaaa.");
            }
        }

        static bool DataValida(string texto)
        {
            if (texto.Length != 10) return false;
            if (texto[2] != '/' || texto[5] != '/') return false;

            int dia, mes, ano;

            if (!int.TryParse(texto.Substring(0, 2), out dia)) return false;
            if (!int.TryParse(texto.Substring(3, 2), out mes)) return false;
            if (!int.TryParse(texto.Substring(6, 4), out ano)) return false;

            if (ano < 1900 || ano > DateTime.Now.Year) return false;
            if (mes < 1 || mes > 12) return false;
            if (dia < 1 || dia > 31) return false;

            return true;
        }


        // Função principal do programa
        static void Main(string[] args)
        {
            Console.WriteLine("==========================================");
            Console.WriteLine("  SISTEMA DE REGISTO ELETRÓNICO DE UTENTES");
            Console.WriteLine("==========================================");

            if (!FazerLogin())
            {
                Console.WriteLine("Número máximo de tentativas excedido.");
                Console.WriteLine("Programa encerrado.");
                return;
            }

            int opcao = -1;
            while (opcao != 0)
            {
                Console.Clear();
                MostrarMenu();
                opcao = LerInteiro("Escolha uma opção: ");

                switch (opcao)
                {
                    case 1:
                        if (TemPermissao(1)) RegistarUtente();
                        break;
                    case 2:
                        ListarUtentes();
                        break;
                    case 3:
                        PesquisarUtente();
                        break;
                    case 4:
                        if (TemPermissao(4)) RegistarConsulta();
                        break;
                    case 5:
                        if (TemPermissao(5)) VerHistorico();
                        break;
                    case 0:
                        Console.WriteLine("Sessão terminada. Até breve!");
                        break;
                    default:
                        Console.WriteLine("Opção inválida.");
                        break;
                }

                if (opcao != 0)
                {
                    Console.Write("Prima Enter para continuar...");
                    Console.ReadLine();
                }
            }

        }
    }
}

