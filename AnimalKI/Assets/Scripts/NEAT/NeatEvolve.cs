using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;


//stoped at randomNeurons
public interface INeatInputs
{ }

public interface INeatOutputs
{ }

public class Specie
{
	public float topFitness = 0;
	public float staleness = 0;
	public Genome[] genomes;
	public float averageFitness = 0;
}

public class Genome
{
    public List<Gene> genes = new List<Gene>();
    public float fitness = 0;
    public float adjustedFitness = 0;
    public NeatNetwork network;
    public int maxNeuron = 0;
    public int globalRank = 0;
    public MutationRates mutationRates;

    public struct MutationRates
    {
        public float connections;
        public float link;
        public float bias;
        public float node;
        public float enable;
        public float disable;
    }

    public void SetMutationRates(NeatEvolve ne)
    {
        mutationRates.connections = ne.MutateConnectionsChance;
        mutationRates.link = ne.LinkMutationChance;
        mutationRates.bias = ne.BiasMutationChance;
        mutationRates.node = ne.NodeMutationChance;
        mutationRates.enable = ne.EnableMutationChance;
        mutationRates.disable = ne.DisableMutationChance;
    }

    public static Genome CopyGenome(Genome genome)
    {
        Genome copy = new Genome();
        //copy.genes = new Gene[genome.genes.Count];
        for (int i = 0; i < genome.genes.Count; i++)
        {
            copy.genes[i] = Gene.CopyGene(genome.genes[i]);
        }
            
        copy.fitness = genome.fitness;
        copy.adjustedFitness = genome.adjustedFitness;
        copy.network = genome.network;
        copy.maxNeuron = genome.maxNeuron;
        copy.globalRank = genome.globalRank;
        copy.mutationRates = new MutationRates();
        copy.mutationRates.connections = genome.mutationRates.connections;
        copy.mutationRates.link = genome.mutationRates.link;
        copy.mutationRates.bias = genome.mutationRates.bias;
        copy.mutationRates.node = genome.mutationRates.node;
        copy.mutationRates.enable = genome.mutationRates.enable;
        copy.mutationRates.disable = genome.mutationRates.disable;

        return copy;
    }

    public static Genome CrossOver(Genome g1, Genome g2, NeatEvolve ne)
    {
        if (g2.fitness > g1.fitness)
        {
            var tmpG = g1;
            g1 = g2;
            g2 = tmpG;
        }

        var child = new Genome();
        child.SetMutationRates(ne);

        Dictionary<int, Gene> innovations2 = new Dictionary<int, Gene>();
        for (int i = 0; i < g2.genes.Count; i++)
        {
            var gene = g2.genes[i];
            innovations2[gene.innovation] = gene;
        }

        for (int i = 0; i < g1.genes.Count; i++)
        {
            var gene1 = g1.genes[i];
            var gene2 = innovations2[gene1.innovation];
            if (gene2 != null && Mathf.RoundToInt(UnityEngine.Random.value) == 1 && gene2.enabled)
            {
                child.genes.Add(Gene.CopyGene(gene2));
            }
            else
                child.genes.Add(Gene.CopyGene(gene1));
        }

        child.maxNeuron = Mathf.Max(g1.maxNeuron, g2.maxNeuron);

        return child;
    }
}

public class Gene
{
    public int into = 0;
    public int outo = 0;
    public float weight = 0;
    public bool enabled = true;
    public int innovation = 0;

    public static Gene CopyGene(Gene gene)
    {
        Gene copy = new Gene();
        copy.into = gene.into;
        copy.outo = gene.outo;
        copy.weight = gene.weight;
        copy.enabled = gene.enabled;
        copy.innovation = gene.innovation;

        return copy;
    }
}



public class Neuron
{
    public List<Gene> incoming = new List<Gene>();
    public float value = 0.0f;
}

public class NeatEvolve : MonoBehaviour
{
	INeatInputs[] Inputs;//= InputSize+1
	INeatOutputs[] Outputs;// = #ButtonNames

	public int Population = 300;
	public float DeltaDisjoint = 2.0f;
	public float DeltaWeights = 0.4f;
	public float DeltaThreshold = 1.0f;

	public int StaleSpecies = 15;

	public float MutateConnectionsChance = 0.25f;
	public float PerturbChance = 0.90f;
	public float CrossoverChance = 0.75f;
	public float LinkMutationChance = 2.0f;
	public float NodeMutationChance = 0.50f;
	public float BiasMutationChance = 0.40f;
	public float StepSize = 0.1f;
	public float DisableMutationChance = 0.4f;
	public float EnableMutationChance = 0.2f;

	public float TimeoutConstant = 20f;

	public int MaxNodes = 1000000;

	struct Pool
	{
		public Specie[] species;
		public int generation;
		public int innovation; //???
		public int currentSpecies;
		public int currentGenome;
		public int currentFrame;
		public float maxFitness;

		public Pool(INeatOutputs[] outputs)
		{
			species = null;
			generation = 0;
			innovation = outputs.Length;
			currentSpecies = 1;
			currentGenome = 1;
			currentFrame = 0;
			maxFitness = 0;
		}
	}
	
	//TODO:
	INeatInputs[] GetInputs()
	{
		return null;
	}

	//aktivierungs funktion
	float Sigmoid(float x)
	{
		return 2 / (1 + Mathf.Exp(-4.9f * x)) - 1;
	}


	//TODO: What is an inovation
	void NewInovation(Pool pool)
	{
		pool.innovation += 1;        
	}

    Genome BasicGenome()
    {
        Genome genome = new Genome();
        genome.SetMutationRates(this);
        int innovation = 1; //???

        genome.maxNeuron = Inputs.Length;
        Mutate(genome);

        return genome;          
    }

    void GenerateNeatNetwork(Genome genome)
    {
        NeatNetwork network = new NeatNetwork();
        network.neurons = null;
        List<Neuron> _neurons = new List<Neuron>();
        for (int i = 0; i < Inputs.Length; i++)
        {
            _neurons.Add(new Neuron());
        }
        for (int o = 0; o < Outputs.Length; o++)
        {
            _neurons.Add(new Neuron());
        }

        //Array.Sort(genome.genes, (a, b) => { return a.outo < b.outo ? a.outo : b.outo; }); //Could be buggy
        List<Gene> tmp = genome.genes.OrderBy(o => o.outo).ToList();
        genome.genes = new List<Gene>(tmp);

        for (int i = 0; i < genome.genes.Count; i++)
        {
            var gene = genome.genes[i];
            if (gene.enabled)
            {
                if (network.neurons[gene.outo] == null)
                    network.neurons[gene.outo] = new Neuron();

                var neuron = network.neurons[gene.outo];
                neuron.incoming.Add(gene);

                if (network.neurons[gene.into] == null)
                {
                    network.neurons[gene.into] = new Neuron();
                }
            }
        }

        genome.network = network;
    }

    //TODO:
    INeatOutputs[] EvaluateNetwork(NeatNetwork network, INeatInputs[] inputs)
    {
        return null;
    }

    private void Mutate(Genome genome)
    {
        throw new NotImplementedException();
    }


}

public class NeatNetwork
{
    public Neuron[] neurons;
}


