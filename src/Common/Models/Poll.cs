﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using BioEngine.Common.Base;
using Newtonsoft.Json;

namespace BioEngine.Common.Models
{
    [Table("be_poll")]
    public class Poll : BaseModel<int>
    {
        private List<PollResultsEntry> _results;

        private bool _voted;

        [Key]
        [Column("poll_id")]
        [JsonProperty]
        public override int Id { get; set; }

        public string Question { get; set; }

        [Column("startdate")]
        public int StartDate { get; set; }

        [Column("options")]
        public string OptionsJson { get; set; }

        [Column("votes")]
        public string VotesJson { get; set; }

        [JsonProperty]
        public int NumChoices { get; set; }
        [JsonProperty]
        public int Multiple { get; set; }

        [Column("onoff")]
        [JsonProperty]
        public int OnOff { get; set; }

        [JsonProperty]
        public List<PollResultsEntry> Results
        {
            get
            {
                if (_results != null) return _results;
                _results = new List<PollResultsEntry>();
                var votes = Votes;
                var all = votes.Sum(vote => int.Parse(vote.Value));

                foreach (var option in Options)
                {
                    var optVotes = int.Parse(votes.FirstOrDefault(x => x.Key == "opt_" + option.Id).Value);
                    _results.Add(new PollResultsEntry
                    {
                        Id = option.Id,
                        Text = option.Text,
                        Result = all > 0 ? Math.Round(optVotes / (double) all, 4) : 0
                    });
                }
                return _results;
            }
        }

        [JsonProperty]
        public List<PollOption> Options => JsonConvert.DeserializeObject<List<PollOption>>(OptionsJson);

        [NotMapped]
        public Dictionary<string, string> Votes
        {
            get => JsonConvert.DeserializeObject<Dictionary<string, string>>(VotesJson);
            set => VotesJson = JsonConvert.SerializeObject(value);
        }

        public void SetVoted()
        {
            _voted = true;
        }

        public bool IsVoted()
        {
            return _voted;
        }
    }

    public struct PollOption
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }
    }

    public struct PollResultsEntry
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public double Result { get; set; }
    }
}