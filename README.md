# Sistema de Registo Eletrónico de Utentes

Aplicação de consola em C# para registar utentes de unidades de saúde, associar cada registo a um número único e, quando aplicável, ao NIF, e guardar o histórico clínico das consultas. O acesso às funcionalidades depende do perfil de quem entra no sistema.

O projeto foi desenvolvido na disciplina de Linguagem de Programação VII, da Licenciatura em Engenharia Informática da Universidade Kimpa Vita, a partir de um enunciado em que o Ministério da Saúde propõe modernizar o registo, a identificação e o acompanhamento dos pacientes. Foi escrito só com as noções básicas da linguagem: variáveis, arrays, decisões, ciclos e métodos. Não há classes próprias, listas, ficheiros nem base de dados.

## Funcionalidades

- Registo de utentes com nome, data de nascimento, sexo, telefone e morada.
- Número de registo gerado automaticamente (UT0001, UT0002 e assim por diante).
- Associação opcional ao NIF, com recusa de NIF já existente.
- Registo de consultas com data automática, profissional, diagnóstico e outras informações clínicas.
- Consulta do histórico clínico de cada utente.
- Pesquisa de utentes pelo número de registo ou pelo NIF.
- Login com três perfis de acesso.
- Validação das entradas: campos obrigatórios, formato da data, sexo e opções do menu.

## Perfis de acesso

O login é pedido ao iniciar o programa, com até três tentativas. As contas de teste usam todas a senha `1234`.

| Perfil | Login | Registar utente | Listar e pesquisar | Registar consulta | Ver histórico |
| --- | --- | --- | --- | --- | --- |
| Recepção | `recepcao` | Sim | Sim | Não | Não |
| Médico | `medico` | Sim | Sim | Sim | Sim |
| Enfermeiro | `enfermeiro` | Não | Sim | Não | Sim |

A listagem e a pesquisa mostram apenas dados de identificação. Os dados clínicos ficam restritos a médicos e enfermeiros.

## Requisitos

- SDK do .NET 6 ou superior. O código foi testado com o .NET 10.
- Projeto com `ImplicitUsings` ativo, que é o padrão dos projetos de consola criados a partir do .NET 6. O `Program.cs` não declara `using System;`.

## Como executar

Na pasta do projeto, execute:

```
dotnet run
```

Para criar o projeto do zero a partir do código deste repositório:

```
dotnet new console -n RegistoUtentes
cd RegistoUtentes
```

Depois substitua o `Program.cs` gerado pelo deste repositório e execute `dotnet run`.

## Utilização

Depois do login, o menu principal mostra as opções disponíveis:

```
------------- MENU PRINCIPAL -------------
Utilizador: Dr. Médico
1. Registar novo utente
2. Listar utentes
3. Pesquisar utente (Nº de registo ou NIF)
4. Registar consulta
5. Consultar histórico clínico
0. Sair
------------------------------------------
```

O ecrã é limpo antes de cada menu e de cada operação. Depois de cada operação, o programa pede Enter para continuar, para que o resultado possa ser lido antes de o menu voltar.

Se um perfil escolher uma opção que não lhe é permitida, o programa mostra a mensagem de acesso negado e volta ao menu.

## Estrutura do código

Todo o programa está num único ficheiro, `Program.cs`.

Os dados ficam em arrays paralelos: cada campo tem o seu array, e a mesma posição em todos eles pertence ao mesmo utente. As consultas guardam o número de registo do utente a que pertencem, e é essa ligação que permite montar o histórico. A capacidade é de 100 utentes e 500 consultas.

Principais métodos:

- `Main`: mostra o cabeçalho, faz o login e executa o ciclo do menu.
- `FazerLogin` e `TemPermissao`: autenticação e controlo de acesso por perfil.
- `RegistarUtente`, `ListarUtentes` e `PesquisarUtente`: gestão dos utentes.
- `RegistarConsulta` e `VerHistorico`: gestão do histórico clínico.
- `LerTextoObrigatorio`, `LerInteiro`, `LerSexo`, `LerData` e `DataValida`: leitura e validação das entradas.

## Limitações

- Os dados existem só em memória. Ao fechar o programa, tudo se perde.
- As contas e as senhas estão escritas no código, e a senha aparece no ecrã ao ser digitada.
- A validação da data confere o intervalo de dia e mês, mas não o calendário. A data 31/02/2000, por exemplo, seria aceite.
- O NIF só é conferido quanto à repetição e a não estar vazio. Não há validação do formato.
- A pesquisa percorre os arrays do início ao fim, o que serve para poucas centenas de registos.

Dados de saúde exigem proteção especial. Antes de qualquer uso real, o sistema precisaria de armazenamento persistente, senhas protegidas e registo de quem acede a cada ficha.

## Autor

João José Antônio da Silva  
Licenciatura em Engenharia Informática, Universidade Kimpa Vita, Uíge, Angola
