	class Graph
	{
		internal Dictionary<string, List<(string, int)>> smezhList = new();
		internal bool wheighted;
		internal bool oriented;
		public Graph(Dictionary<string, List<(string, int)>> vertices, bool wheighted, bool oriented)
	{
        this.smezhList = vertices;
        this.wheighted = wheighted;
        this.oriented = oriented;
    }
		public Graph()
		{
			smezhList = new Dictionary<string, List<(string, int)>>();
			wheighted = false;
			oriented = false;
		}
		public Graph(Graph graph)
		{
			this.smezhList = graph.smezhList;
			this.oriented = graph.oriented;
			this.wheighted = graph.wheighted;
		}
		public Graph(string path)
		{
			string[] matrix = File.ReadAllLines(path);
			int[][] SmezhMatrix = new int[matrix.Length][];
			for (int i = 0; i < matrix.Length; i++)
			{
				SmezhMatrix[i] = new int[matrix[i].Length];
				string[] row = matrix[i].Split(",");
				for (int j = 0; j < row.Length; j++)
				{
					SmezhMatrix[i][j] = int.Parse(row[j]);
				}
			}
			for (int i = 0; i < SmezhMatrix.Length; i++)
			{
				AddVertex($"V{i + 1}");
				for (int j = 0; j < SmezhMatrix.Length; j++)
				{
					if (SmezhMatrix[i][j] > 0) smezhList[$"V{i + 1}"].Add(($"V{j + 1}", SmezhMatrix[i][j])); ;
					if (!wheighted && (SmezhMatrix[i][j] != 1 || SmezhMatrix[i][j] != -1)) wheighted = true;
					if (!oriented && SmezhMatrix[i][j] < 0) oriented = true;
				}
			}
		}
		public static Graph CompleteGraph(Graph graph)
		{
			Dictionary<string, List<(string, int)>> AdjList = new();
			foreach (var vertexK in graph.smezhList.Keys)
			{
				AdjList.Add(vertexK, new List<(string, int)>());
				foreach (var VertexV in graph.smezhList.Keys)
				{
					if (vertexK != VertexV) AdjList[vertexK].Add((VertexV, 1));
				}
			}
			return new Graph(AdjList, graph.wheighted, graph.oriented);
		}
		public static Graph ComplementGraph(Graph graph)
		{
			Dictionary<string, List<(string, int)>> AdjList = new();
			foreach (var vertexK in graph.smezhList.Keys)
			{
				AdjList.Add(vertexK, new List<(string, int)>());
				foreach (var VertexV in graph.smezhList.Keys)
				{
					if (!graph.smezhList[vertexK].Any(x => x.Item1 == VertexV) && VertexV != vertexK) AdjList[vertexK].Add((VertexV, 1));
				}
			}
			return new Graph(AdjList, graph.wheighted, graph.oriented);
		}
		public static Graph? UnionGraph(Graph graph1, Graph graph2)
		{
			Dictionary<string, List<(string, int)>> AdjList = new();
			if (graph1.smezhList.Keys.Intersect(graph2.smezhList.Keys).Any())
				return null;
			foreach (var item in graph1.smezhList)
				AdjList.Add(item.Key, item.Value);
			foreach (var item in graph2.smezhList)
				AdjList.Add(item.Key, item.Value);
			return new Graph(AdjList, false, false);
		}
		public static Graph? IntersectionGraph(Graph graph1, Graph graph2)
		{
			Dictionary<string, List<(string, int)>> AdjList = new();
			if (graph1.smezhList.Keys.Intersect(graph2.smezhList.Keys).Any())
				return null;
			foreach (var item in graph1.smezhList)
			{
				AdjList.Add(item.Key, item.Value);
			}
			foreach (var item in graph2.smezhList)
			{
				AdjList.Add(item.Key, item.Value);
			}
			Graph gr = new Graph(AdjList, false, false);
			foreach (var item1 in graph1.smezhList)
			{
				foreach (var item2 in graph2.smezhList)
				{
					gr.AddEdge(item1.Key, item2.Key);
				}
			}
			return gr;
		}
		public void AddVertex(string vertex)
		{
			smezhList.Add(vertex, new List<(string, int)>());
		}
		public void RemoveVertex(string vertex)
		{
			if (wheighted)
			{
				foreach (var vertices in smezhList.Values)
				{
					foreach (var vert in vertices)
					{
						if (vert.Item1 == vertex)
						{
							vertices.Remove(vert);
						}
					}
				}
			}
			else
				foreach (var vertices in smezhList.Values) vertices.Remove((vertex, 1));
			smezhList.Remove(vertex);
		}
		public void AddEdge(string vertex1, string vertex2, int weight = 1)
		{
			if (!(smezhList[vertex1].Contains((vertex2, weight)) && (smezhList[vertex2].Contains((vertex1, weight)))))
			{
				if (vertex1 == vertex2 || oriented) smezhList[vertex1].Add((vertex2, weight));
				else
				{
					smezhList[vertex1].Add((vertex2, weight));
					smezhList[vertex2].Add((vertex1, weight));
				}
			}
		}
		public void RemoveEdge(string vertex1, string vertex2)
		{
			foreach (var vert in smezhList[vertex1])
			{
				if (vert.Item1 == vertex2)
				{
					smezhList[vertex1].Remove(vert);
				}
			}
			foreach (var vert in smezhList[vertex2])
			{
				if (vert.Item1 == vertex1)
				{
					smezhList[vertex2].Remove(vert);
				}
			}
		}
		public int[][] CreateAdjacencyMatrix()
		{
			List<string> vertexList = smezhList.Keys.ToList();
			int[][] matrix = new int[smezhList.Keys.Count][];
			foreach (string v in vertexList) matrix[vertexList.IndexOf(v)] = new int[smezhList.Keys.Count];
			foreach (string v in vertexList)
			{
				foreach (var adjVert in smezhList[v])
				{
					matrix[vertexList.IndexOf(v)][vertexList.IndexOf(adjVert.Item1)] = adjVert.Item2;
					if (oriented) matrix[vertexList.IndexOf(adjVert.Item1)][vertexList.IndexOf(v)] = -adjVert.Item2;
				}
			}
			return matrix;
		}
		public void PrintSmezhMatrix(int[][] matrix)
		{
			for (int i = 0; i < matrix.Length; i++)
			{
				for (int j = 0; j < matrix.Length; j++)
				{
					Console.Write($"{matrix[i][j]} ");
				}
				Console.WriteLine();
			}
		}
		public void PrintSmezhList()
		{
			for (int i = 0; i < smezhList.Count; i++)
			{
				Console.Write($"{smezhList.ElementAt(i).Key} : ");
				foreach (var item in smezhList[smezhList.ElementAt(i).Key])
				{
					Console.Write($"{item.Item1} ");
				}
				Console.WriteLine();
			}
		}
		public void WriteSmezhMatrixToFile(string path)
		{
			int[][] smezhMatrix = CreateAdjacencyMatrix();
			string result = "";
			for (int i = 0; i < smezhMatrix.Length; i++)
			{
				for (int j = 0; j < smezhMatrix.Length; j++)
				{
					result += ($"{smezhMatrix[i][j]} ");
				}
				result += "\n";
			}
			File.WriteAllText(path, result);
		}
		public void WriteSmezhListToFile(string path)
		{
			string res = "";
			for (int i = 0; i < smezhList.Count; i++)
			{
				Console.Write($"{smezhList.ElementAt(i).Key} : ");
				foreach (var item in smezhList[smezhList.ElementAt(i).Key])
				{
					res += ($"{item.Item1} ");
				}
				res += "\n";
			}
			File.WriteAllText(path, res);
		}
	}
class UI { 
	static void UIgraph(Graph graph)
	{
		Console.WriteLine(
			"1. Добавить вершину\n" +
			"2. Добавить ребро\n" +
			"3. Удалить вершину\n" +
			"4. Удалить ребро\n" +
			"5. Вывести список смежности\n" +
			"6. Сохранить список смежности в файл\n" +
			"7. Выход");
		string id;
		List<string> idList = new();

		while (true)
		{
			Console.WriteLine("Выберите номер операции:");
			switch (Console.ReadLine())
			{
				case "1":
					Console.WriteLine("Введите название новой вершины");
					id = Console.ReadLine();
					if (!graph.smezhList.ContainsKey(id)) graph.AddVertex(new string(id));
					else Console.WriteLine("Вершина уже существует");
					break;
				case "2":
					if (graph.wheighted)
					{
						Console.WriteLine("Введите вершины, между которыми создается ребро и вес ребра(v1 v2 wheight)");
						idList = Console.ReadLine().Split(" ").ToList();
						graph.AddEdge(idList[0], idList[1], Convert.ToInt32(idList[2]));
					}
					else
					{
						Console.WriteLine("Введите вершины, между которыми создается ребро(v1 v2)");
						idList = Console.ReadLine().Split(" ").ToList();
						graph.AddEdge(idList[0], idList[1]);
					}
					break;
				case "3":
					Console.WriteLine("Введите название удаляемой вершины");
					id = Console.ReadLine();
					if (graph.smezhList.ContainsKey(id)) graph.RemoveVertex(id);
					else Console.WriteLine("вершины не существует");
					break;
				case "4":
					Console.WriteLine("Введите вершины, между которыми удаляется ребро(v1 v2)");
					idList = Console.ReadLine().Split(" ").ToList();
					graph.RemoveEdge(idList[0], idList[1]);
					break;
				case "5":
					graph.PrintSmezhList();
					break;
				case "6"://граф в файл
					Console.WriteLine("Введите путь до файла:");
					graph.WriteSmezhListToFile(Console.ReadLine());
					break;
				case "7":
					return;
				default:
					Console.WriteLine("Выбрана несуществующая операция");
					break;
			}
		}
	}
}